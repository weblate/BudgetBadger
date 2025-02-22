﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using BudgetBadger.Core.DataAccess;
using BudgetBadger.Core.Logic;
using BudgetBadger.DataAccess.Sqlite;
using BudgetBadger.Logic;
using BudgetBadger.Forms.Payees;
using BudgetBadger.Forms.Views;
using BudgetBadger.Forms.ViewModels;
using DryIoc;
using Prism;
using Prism.DryIoc;
using Prism.Ioc;
using BudgetBadger.Forms.Accounts;
using BudgetBadger.Forms.Envelopes;
using BudgetBadger.Forms.Transactions;
using BudgetBadger.Core.Sync;
using BudgetBadger.Core.Files;
using BudgetBadger.FileSyncProvider.Dropbox;
using BudgetBadger.Forms.Settings;
using BudgetBadger.Core.Settings;
using Prism.AppModel;
using BudgetBadger.Forms.Enums;
using System.IO;
using Prism.Services;
using BudgetBadger.Forms.Reports;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Threading.Tasks;
using BudgetBadger.Models;
using Microsoft.Data.Sqlite;
using BudgetBadger.Core.LocalizedResources;
using System.Globalization;
using BudgetBadger.Core.Utilities;
using Prism.Navigation;
using Prism.Events;
using BudgetBadger.Forms.Events;
using System.Threading;
using BudgetBadger.Forms.UserControls;
using System.Linq;
using BudgetBadger.Forms.Style;
using BudgetBadger.Forms.Authentication;
using BudgetBadger.Core.Authentication;
using BudgetBadger.FileSyncProvider.Dropbox.Authentication;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace BudgetBadger.Forms
{
    public partial class App : PrismApplication
    {
        /* 
         * The Xamarin Forms XAML Previewer in Visual Studio uses System.Activator.CreateInstance.
         * This imposes a limitation in which the App class must have a default constructor. 
         * App(IPlatformInitializer initializer = null) cannot be handled by the Activator.
         */
        public App() : this(null) { }

        public App(IPlatformInitializer initializer) : base(initializer) { }

        private Timer _syncTimer;

        protected override async void OnInitialized()
        {
            InitializeComponent();

            var settings = Container.Resolve<ISettings>();
            var appOCount = await settings.GetValueOrDefaultAsync(AppSettings.AppOpenedCount);
            int.TryParse(appOCount, out int appOpenedCount);
            if (appOpenedCount > 0)
            {
                if (Device.Idiom == TargetIdiom.Desktop)
                {
                    var parameters = new NavigationParameters
                    {
                        { PageParameter.PageName, "EnvelopesPage" }
                    };
                    await NavigationService.NavigateAsync("/MainPage/NavigationPage/EnvelopesPage", parameters);
                }
                else
                {
                    await NavigationService.NavigateAsync("MainPage");
                }
            }
            else
            {
                if (Device.Idiom == TargetIdiom.Desktop)
                {
                    var parameters = new NavigationParameters
                    {
                        { PageParameter.PageName, "AccountsPage" }
                    };
                    await NavigationService.NavigateAsync("/MainPage/NavigationPage/AccountsPage", parameters);
                }
                else
                {
                    await NavigationService.NavigateAsync("MainPage?selectedTab=AccountsPage");
                }
            }

            _syncTimer = new Timer(_ => Task.Run(async () => await Sync()).FireAndForget());

            SetAppTheme(Application.Current.RequestedTheme);
            await SetAppearanceSize();
            await SetLocale();

            Application.Current.RequestedThemeChanged += (s, a) => 
            {
                SetAppTheme(a.RequestedTheme);
            };

            var eventAggregator = Container.Resolve<IEventAggregator>();
            eventAggregator.GetEvent<AccountDeletedEvent>().Subscribe(x => ResetSyncTimer());
            eventAggregator.GetEvent<AccountHiddenEvent>().Subscribe(x => ResetSyncTimer());
            eventAggregator.GetEvent<AccountSavedEvent>().Subscribe(x => ResetSyncTimer());
            eventAggregator.GetEvent<AccountUnhiddenEvent>().Subscribe(x => ResetSyncTimer());

            eventAggregator.GetEvent<BudgetSavedEvent>().Subscribe(x => ResetSyncTimer());
            eventAggregator.GetEvent<EnvelopeDeletedEvent>().Subscribe(x => ResetSyncTimer());
            eventAggregator.GetEvent<EnvelopeHiddenEvent>().Subscribe(x => ResetSyncTimer());
            eventAggregator.GetEvent<EnvelopeUnhiddenEvent>().Subscribe(x => ResetSyncTimer());

            eventAggregator.GetEvent<EnvelopeGroupDeletedEvent>().Subscribe(x => ResetSyncTimer());
            eventAggregator.GetEvent<EnvelopeGroupHiddenEvent>().Subscribe(x => ResetSyncTimer());
            eventAggregator.GetEvent<EnvelopeGroupSavedEvent>().Subscribe(x => ResetSyncTimer());
            eventAggregator.GetEvent<EnvelopeGroupUnhiddenEvent>().Subscribe(x => ResetSyncTimer());

            eventAggregator.GetEvent<PayeeDeletedEvent>().Subscribe(x => ResetSyncTimer());
            eventAggregator.GetEvent<PayeeHiddenEvent>().Subscribe(x => ResetSyncTimer());
            eventAggregator.GetEvent<PayeeSavedEvent>().Subscribe(x => ResetSyncTimer());
            eventAggregator.GetEvent<PayeeUnhiddenEvent>().Subscribe(x => ResetSyncTimer());

            eventAggregator.GetEvent<SplitTransactionSavedEvent>().Subscribe(ResetSyncTimer);
            eventAggregator.GetEvent<SplitTransactionStatusUpdatedEvent>().Subscribe(x => ResetSyncTimer());
            eventAggregator.GetEvent<TransactionDeletedEvent>().Subscribe(x => ResetSyncTimer());
            eventAggregator.GetEvent<TransactionSavedEvent>().Subscribe(x => ResetSyncTimer());
            eventAggregator.GetEvent<TransactionStatusUpdatedEvent>().Subscribe(x => ResetSyncTimer());
        }

        protected async override void OnStart()
        {
            await UpgradeApp();

            // tracking number of times app opened
            var settings = Container.Resolve<ISettings>();
            var appOCount = await settings.GetValueOrDefaultAsync(AppSettings.AppOpenedCount);
            int.TryParse(appOCount, out int appOpenedCount);
            appOpenedCount++;
            await settings.AddOrUpdateValueAsync(AppSettings.AppOpenedCount, appOpenedCount.ToString());

            ResetSyncTimerAtStartOrResume();
        }

        protected override void OnResume()
        {
            ResetSyncTimerAtStartOrResume();
        }

        protected override Rules CreateContainerRules()
        {
            return base.CreateContainerRules().WithDefaultIfAlreadyRegistered(IfAlreadyRegistered.Replace);
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            var timer = Stopwatch.StartNew();

            var container = containerRegistry.GetContainer();

            container.Register<ISettings, SecureStorageSettings>();
            container.Register<IResourceContainer, ResourceContainer>();

            var appDataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BudgetBadger");
            Directory.CreateDirectory(appDataDirectory);

            var dataDirectory = Path.Combine(appDataDirectory, "data");
            Directory.CreateDirectory(dataDirectory);
            var syncDirectory = Path.Combine(appDataDirectory, "sync");
            if (Directory.Exists(syncDirectory))
            {
                Directory.Delete(syncDirectory, true);
            }
            Directory.CreateDirectory(syncDirectory);

            //default dataaccess
            var defaultConnectionString = "Data Source=" + Path.Combine(dataDirectory, "default.bb");
            container.UseInstance(defaultConnectionString, serviceKey: "defaultConnectionString");
            container.Register<IAccountDataAccess>(made: Made.Of(() => new AccountSqliteDataAccess(Arg.Of<string>("defaultConnectionString"))));
            container.Register<IPayeeDataAccess>(made: Made.Of(() => new PayeeSqliteDataAccess(Arg.Of<string>("defaultConnectionString"))));
            container.Register<IEnvelopeDataAccess>(made: Made.Of(() => new EnvelopeSqliteDataAccess(Arg.Of<string>("defaultConnectionString"))));
            container.Register<ITransactionDataAccess>(made: Made.Of(() => new TransactionSqliteDataAccess(Arg.Of<string>("defaultConnectionString"))));

            //default logic
            container.Register<ITransactionLogic, TransactionLogic>();
            container.Register<IAccountLogic, AccountLogic>();
            container.Register<IPayeeLogic, PayeeLogic>();
            container.Register<IEnvelopeLogic, EnvelopeLogic>();
            container.Register<IReportLogic, ReportLogic>();

            //sync dataaccess
            var syncConnectionString = "Data Source=" + Path.Combine(syncDirectory, "default.bb");
            container.UseInstance(syncConnectionString, serviceKey: "syncConnectionString");
            container.Register<IAccountDataAccess>(made: Made.Of(() => new AccountSqliteDataAccess(Arg.Of<string>("syncConnectionString"))), serviceKey: "syncAccountDataAccess");
            container.Register<IPayeeDataAccess>(made: Made.Of(() => new PayeeSqliteDataAccess(Arg.Of<string>("syncConnectionString"))), serviceKey: "syncPayeeDataAccess");
            container.Register<IEnvelopeDataAccess>(made: Made.Of(() => new EnvelopeSqliteDataAccess(Arg.Of<string>("syncConnectionString"))), serviceKey: "syncEnvelopeDataAccess");
            container.Register<ITransactionDataAccess>(made: Made.Of(() => new TransactionSqliteDataAccess(Arg.Of<string>("syncConnectionString"))), serviceKey: "syncTransactionDataAccess");

            //sync directory for filesyncproviders
            container.Register<IDirectoryInfo>(made: Made.Of(() => new LocalDirectoryInfo(syncDirectory)));

            //sync logics
            container.Register<IAccountSyncLogic>(made: Made.Of(() => new AccountSyncLogic(Arg.Of<IAccountDataAccess>(),
                                                                                           Arg.Of<IAccountDataAccess>("syncAccountDataAccess"))));
            container.Register<IPayeeSyncLogic>(made: Made.Of(() => new PayeeSyncLogic(Arg.Of<IPayeeDataAccess>(),
                                                                                       Arg.Of<IPayeeDataAccess>("syncPayeeDataAccess"))));
            container.Register<IEnvelopeSyncLogic>(made: Made.Of(() => new EnvelopeSyncLogic(Arg.Of<IEnvelopeDataAccess>(),
                                                                                             Arg.Of<IEnvelopeDataAccess>("syncEnvelopeDataAccess"))));
            container.Register<ITransactionSyncLogic>(made: Made.Of(() => new TransactionSyncLogic(Arg.Of<ITransactionDataAccess>(),
                                                                                                   Arg.Of<ITransactionDataAccess>("syncTransactionDataAccess"))));

            container.Register<IFileSyncProvider>(made: Made.Of(() => new DropboxFileSyncProvider(Arg.Of<ISettings>(),
                                                                                                  Arg.Of<string>("dropBoxAppKey"))), serviceKey: SyncMode.DropboxSync);



            container.Register<ISyncFactory>(made: Made.Of(() => new SyncFactory(Arg.Of<IResourceContainer>(),
                                                                          Arg.Of<ISettings>(),
                                                                          Arg.Of<IDirectoryInfo>(),
                                                                          Arg.Of<IAccountSyncLogic>(),
                                                                          Arg.Of<IPayeeSyncLogic>(),
                                                                          Arg.Of<IEnvelopeSyncLogic>(),
                                                                          Arg.Of<ITransactionSyncLogic>(),
                                                                          Arg.Of<KeyValuePair<string, IFileSyncProvider>[]>(),
                                                                          Arg.Of<IDropboxAuthentication>(),
                                                                          Arg.Of<IPageDialogService>())));

            container.Register(made: Made.Of(() => StaticSyncFactory.CreateSyncAsync(Arg.Of<ISettings>(),
                                                                          Arg.Of<IDirectoryInfo>(),
                                                                          Arg.Of<IAccountSyncLogic>(),
                                                                          Arg.Of<IPayeeSyncLogic>(),
                                                                          Arg.Of<IEnvelopeSyncLogic>(),
                                                                          Arg.Of<ITransactionSyncLogic>(),
                                                                          Arg.Of<KeyValuePair<string, IFileSyncProvider>[]>())));

            if (Device.RuntimePlatform != Device.UWP)
            {
                container.Register<IWebAuthenticator, WebAuthenticator>();
            }
            container.UseInstance(AppSecrets.DropBoxAppKey, serviceKey: "dropBoxAppKey");
            container.Register<IDropboxAuthentication>(made: Made.Of(() => new DropboxAuthentication(
                Arg.Of<IWebAuthenticator>(),
                Arg.Of<string>("dropBoxAppKey"))));

            containerRegistry.RegisterForNavigationOnIdiom<MainPage, MainPageViewModel>(desktopView: typeof(MainDesktopPage), tabletView: typeof(MainTabletPage));

            containerRegistry.RegisterForNavigation<MyPage>("NavigationPage");
            containerRegistry.RegisterForNavigationOnIdiom<AccountsPage, AccountsPageViewModel>(desktopView: typeof(AccountsDetailedPage), tabletView: typeof(AccountsDetailedPage));
            containerRegistry.RegisterForNavigation<AccountSelectionPage, AccountSelectionPageViewModel>();
            containerRegistry.RegisterForNavigationOnIdiom<AccountInfoPage, AccountInfoPageViewModel>(desktopView: typeof(AccountInfoDetailedPage), tabletView: typeof(AccountInfoDetailedPage));
            containerRegistry.RegisterForNavigation<AccountEditPage, AccountEditPageViewModel>();
            containerRegistry.RegisterForNavigationOnIdiom<AccountReconcilePage, AccountReconcilePageViewModel>(desktopView: typeof(AccountReconcileDetailedPage), tabletView: typeof(AccountReconcileDetailedPage));
            containerRegistry.RegisterForNavigation<HiddenAccountsPage, HiddenAccountsPageViewModel>();
            containerRegistry.RegisterForNavigationOnIdiom<PayeesPage, PayeesPageViewModel>(desktopView: typeof(PayeesDetailedPage), tabletView: typeof(PayeesDetailedPage));
            containerRegistry.RegisterForNavigation<PayeeSelectionPage, PayeeSelectionPageViewModel>();
            containerRegistry.RegisterForNavigationOnIdiom<PayeeInfoPage, PayeeInfoPageViewModel>(desktopView: typeof(PayeeInfoDetailedPage), tabletView: typeof(PayeeInfoDetailedPage));
            containerRegistry.RegisterForNavigation<PayeeEditPage, PayeeEditPageViewModel>();
            containerRegistry.RegisterForNavigation<HiddenPayeesPage, HiddenPayeesPageViewModel>();
            containerRegistry.RegisterForNavigationOnIdiom<EnvelopesPage, EnvelopesPageViewModel>(desktopView: typeof(EnvelopesDetailedPage), tabletView: typeof(EnvelopesDetailedPage));
            containerRegistry.RegisterForNavigation<EnvelopeSelectionPage, EnvelopeSelectionPageViewModel>();
            containerRegistry.RegisterForNavigationOnIdiom<EnvelopeInfoPage, EnvelopeInfoPageViewModel>(desktopView: typeof(EnvelopeInfoDetailedPage), tabletView: typeof(EnvelopeInfoDetailedPage));
            containerRegistry.RegisterForNavigation<EnvelopeEditPage, EnvelopeEditPageViewModel>();
            containerRegistry.RegisterForNavigation<EnvelopeTransferPage, EnvelopeTransferPageViewModel>();
            containerRegistry.RegisterForNavigation<HiddenEnvelopesPage, HiddenEnvelopesPageViewModel>();
            containerRegistry.RegisterForNavigationOnIdiom<EnvelopeGroupsPage, EnvelopeGroupsPageViewModel>(desktopView: typeof(EnvelopeGroupsDetailedPage), tabletView: typeof(EnvelopeGroupsDetailedPage));
            containerRegistry.RegisterForNavigation<EnvelopeGroupSelectionPage, EnvelopeGroupSelectionPageViewModel>();
            containerRegistry.RegisterForNavigation<EnvelopeGroupEditPage, EnvelopeGroupEditPageViewModel>();
            containerRegistry.RegisterForNavigation<HiddenEnvelopeGroupsPage, HiddenEnvelopeGroupsPageViewModel>();
            containerRegistry.RegisterForNavigation<TransactionEditPage, TransactionEditPageViewModel>();
            containerRegistry.RegisterForNavigationOnIdiom<SplitTransactionPage, SplitTransactionPageViewModel>(desktopView: typeof(SplitTransactionDetailedPage), tabletView: typeof(SplitTransactionDetailedPage));
            containerRegistry.RegisterForNavigation<SettingsPage, SettingsPageViewModel>();
            containerRegistry.RegisterForNavigation<ThirdPartyNoticesPage, ThirdPartyNoticesPageViewModel>();
            containerRegistry.RegisterForNavigation<LicensePage, LicensePageViewModel>();
            containerRegistry.RegisterForNavigation<ReportsPage, ReportsPageViewModel>();
            containerRegistry.RegisterForNavigation<NetWorthReportPage, NetWorthReportPageViewModel>();
            containerRegistry.RegisterForNavigation<EnvelopesSpendingReportPage, EnvelopesSpendingReportPageViewModel>();
            containerRegistry.RegisterForNavigation<EnvelopeTrendsReportPage, EnvelopeTrendsReportsPageViewModel>();
            containerRegistry.RegisterForNavigation<PayeesSpendingReportPage, PayeesSpendingReportPageViewModel>();
            containerRegistry.RegisterForNavigation<PayeeTrendsReportPage, PayeeTrendsReportPageViewModel>();

            timer.Stop();
        }

        void SetAppTheme(OSAppTheme oSAppTheme)
        {
            ICollection<ResourceDictionary> mergedDictionaries = Application.Current.Resources.MergedDictionaries;
            if (mergedDictionaries != null)
            {
                var dictsToRemove = mergedDictionaries.Where(m => (m is LightThemeResources)
                                                               || (m is DarkThemeResources)).ToList();

                foreach (var dict in dictsToRemove)
                {
                    mergedDictionaries.Remove(dict);
                }

                switch (oSAppTheme)
                {
                    case OSAppTheme.Dark:
                        mergedDictionaries.Add(new DarkThemeResources());
                        break;
                    case OSAppTheme.Light:
                    default:
                        mergedDictionaries.Add(new LightThemeResources());
                        break;
                }
            }

            DynamicResourceProvider.Instance.Invalidate();
        }

        async Task SetAppearanceSize()
        {
            var settings = Container.Resolve<ISettings>();

            var currentDimension = await settings.GetValueOrDefaultAsync(AppSettings.AppearanceDimensionSize);

            if (Enum.TryParse(currentDimension, out DimensionSize selectedDimensionSize))
            {
                ICollection<ResourceDictionary> mergedDictionaries = Application.Current.Resources.MergedDictionaries;
                if (mergedDictionaries != null)
                {
                    var dictsToRemove = mergedDictionaries.Where(m => (m is LargeDimensionResources)
                                                                   || (m is MediumDimensionResources)
                                                                   || (m is SmallDimensionResources)).ToList();

                    foreach (var dict in dictsToRemove)
                    {
                        mergedDictionaries.Remove(dict);
                    }

                    switch (selectedDimensionSize)
                    {
                        case DimensionSize.DimensionSizeLarge:
                            mergedDictionaries.Add(new LargeDimensionResources());
                            break;
                        case DimensionSize.DimensionSizeSmall:
                            mergedDictionaries.Add(new SmallDimensionResources());
                            break;
                        case DimensionSize.DimensionSizeMedium:
                        default:
                            mergedDictionaries.Add(new MediumDimensionResources());
                            break;
                    }
                }

                DynamicResourceProvider.Instance.Invalidate();
            }
        }


        async Task SetLocale()
        {
            var localize = Container.Resolve<ILocalize>();
            var settings = Container.Resolve<ISettings>();

            var currentLanguage = await settings.GetValueOrDefaultAsync(AppSettings.Language);
            CultureInfo currentCulture = null;

            if (!String.IsNullOrEmpty(currentLanguage))
            {
                try
                {

                    currentCulture = new CultureInfo(currentLanguage);
                }
                catch (Exception)
                {
                    // culture doesn't exist
                }
            }

            if (currentCulture == null)
            {
                currentCulture = (CultureInfo)localize.GetDeviceCultureInfo().Clone();
            }
            
            
            var currentCurrencyFormat = await settings.GetValueOrDefaultAsync(AppSettings.CurrencyFormat);
            if (!String.IsNullOrEmpty(currentCurrencyFormat))
            {
                try
                {
                    var currencyCulture = new CultureInfo(currentCurrencyFormat);
                    currentCulture.NumberFormat = currencyCulture.NumberFormat;
                }
                catch(Exception)
                {
                    // culture doesn't exist
                }
            }

            localize.SetLocale(currentCulture);
        }

        private void ResetSyncTimerAtStartOrResume()
        {
            _syncTimer.Change(TimeSpan.FromMilliseconds(1), TimeSpan.FromMilliseconds(-1));
        }

        private void ResetSyncTimer()
        {
            _syncTimer.Change(TimeSpan.FromSeconds(5), TimeSpan.FromMilliseconds(-1));
        }

        async Task Sync()
        {
            var syncFactory = Container.Resolve<ISyncFactory>();
            var syncService = await syncFactory.GetSyncServiceAsync();
            var syncResult = await syncService.FullSync();
            if (syncResult.Success)
            {
                await syncFactory.SetLastSyncDateTime(DateTime.Now);
            }
        }

        async Task UpgradeApp()
        {
            await MigrateFromApplicationStoreToSecureStorageSettings();
            var settings = Container.Resolve<ISettings>();
            var appMigrationVersionString = await settings.GetValueOrDefaultAsync(AppSettings.AppMigrationVersion);

            int.TryParse(appMigrationVersionString, out int appMigrationVersion);
            
            switch (appMigrationVersion)
            {
                case 0:
                    await settings.AddOrUpdateValueAsync(AppSettings.AppMigrationVersion, "3");
                    break;
                case 1:
                    await UpgradeAppFromV1ToV2();
                    break;
                case 2:
                    await UpgradeAppFromV2ToV3();
                    break;
                default:
                    break;
            }
        }

        async Task MigrateFromApplicationStoreToSecureStorageSettings()
        {
            var appStore = new ApplicationStore();

            if (appStore.Properties.Any())
            {
                var settings = Container.Resolve<ISettings>();
                foreach (var setting in appStore.Properties)
                {
                    if (setting.Key == "CurrentAppVersion")
                    {
                        int.TryParse(setting.Value.ToString(), out int currentAppVersion);
                        var appMigrationVersion = currentAppVersion + 1;
                        await settings.AddOrUpdateValueAsync(AppSettings.AppMigrationVersion, appMigrationVersion.ToString());
                    }
                    else
                    {
                        await settings.AddOrUpdateValueAsync(setting.Key, setting.Value.ToString());
                    }
                }
                appStore.Properties.Clear();
                await appStore.SavePropertiesAsync();
            }
        }

        async Task UpgradeAppFromV1ToV2()
        {
            var settings = Container.Resolve<ISettings>();
            var syncMode = await settings.GetValueOrDefaultAsync(AppSettings.SyncMode);

            if (syncMode == SyncMode.DropboxSync)
            {
                var dialogService = Container.Resolve<IPageDialogService>();
                var syncFactory = Container.Resolve<ISyncFactory>();
                var resourceContainer = Container.Resolve<IResourceContainer>();
                var loginDropbox = await dialogService.DisplayAlertAsync(resourceContainer.GetResourceString("AlertActionNeeded"),
                    resourceContainer.GetResourceString("AlertDropboxUpgrade"),
                    resourceContainer.GetResourceString("AlertYes"),
                    resourceContainer.GetResourceString("AlertNoThanks"));
                if (loginDropbox)
                {
                    await syncFactory.EnableDropboxCloudSync();
                }
                else
                {
                    await syncFactory.DisableDropboxCloudSync();
                }
            }

            await settings.AddOrUpdateValueAsync(AppSettings.AppMigrationVersion, "1");
        }

        async Task UpgradeAppFromV2ToV3()
        {
            var settings = Container.Resolve<ISettings>();
            var syncMode = await settings.GetValueOrDefaultAsync(AppSettings.SyncMode);
            var refreshToken = await settings.GetValueOrDefaultAsync(DropboxSettings.RefreshToken);

            if (syncMode == SyncMode.DropboxSync && string.IsNullOrEmpty(refreshToken))
            {
                var dialogService = Container.Resolve<IPageDialogService>();
                var syncFactory = Container.Resolve<ISyncFactory>();
                var resourceContainer = Container.Resolve<IResourceContainer>();
                var loginDropbox = await dialogService.DisplayAlertAsync(resourceContainer.GetResourceString("AlertActionNeeded"),
                    resourceContainer.GetResourceString("AlertDropboxUpgrade"),
                    resourceContainer.GetResourceString("AlertYes"),
                    resourceContainer.GetResourceString("AlertNoThanks"));
                if (loginDropbox)
                {
                    await syncFactory.EnableDropboxCloudSync();
                }
                else
                {
                    await syncFactory.DisableDropboxCloudSync();
                }
            }

            await settings.AddOrUpdateValueAsync(AppSettings.AppMigrationVersion, "2");
        }
    }
}
 