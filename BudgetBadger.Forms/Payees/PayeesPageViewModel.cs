﻿using System;
using System.Threading.Tasks;
using System.Windows.Input;
using BudgetBadger.Core.Logic;
using BudgetBadger.Models;
using BudgetBadger.Forms.Enums;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System.Collections.Generic;
using System.Linq;
using Prism.Mvvm;
using Prism.AppModel;
using BudgetBadger.Core.Sync;
using Prism;
using System.Collections.ObjectModel;
using BudgetBadger.Models.Extensions;
using BudgetBadger.Core.LocalizedResources;
using BudgetBadger.Core.Purchase;
using Prism.Events;
using BudgetBadger.Forms.Events;

namespace BudgetBadger.Forms.Payees
{
    public class PayeesPageViewModel : BaseViewModel, INavigatedAware
    {
        readonly Lazy<IResourceContainer> _resourceContainer;
        readonly Lazy<IPayeeLogic> _payeeLogic;
        readonly INavigationService _navigationService;
        readonly IPageDialogService _dialogService;
        readonly Lazy<ISyncFactory> _syncFactory;
        readonly Lazy<IPurchaseService> _purchaseService;
        readonly IEventAggregator _eventAggregator;

        public ICommand SelectedCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        public ICommand RefreshPayeeCommand { get; set; }
        public ICommand AddCommand { get; set; }
        public ICommand SaveSearchCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand AddTransactionCommand { get; set; }
        public Predicate<object> Filter { get => (payee) => _payeeLogic.Value.FilterPayee((Payee)payee, SearchText); }

        bool _needToSync;

        bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        ObservableList<Payee> _payees;
        public ObservableList<Payee> Payees
        {
            get => _payees;
            set => SetProperty(ref _payees, value);
        }

        Payee _selectedPayee;
        public Payee SelectedPayee
        {
            get => _selectedPayee;
            set => SetProperty(ref _selectedPayee, value);
        }

        public bool HasSearchText { get => !string.IsNullOrWhiteSpace(SearchText); }

        string _searchText;
        public string SearchText
        {
            get => _searchText;
            set { SetProperty(ref _searchText, value); RaisePropertyChanged(nameof(HasSearchText)); }
        }

        bool _noPayees;
        public bool NoPayees
        {
            get => _noPayees;
            set => SetProperty(ref _noPayees, value);
        }

        bool _hasPro;
        public bool HasPro
        {
            get => _hasPro;
            set => SetProperty(ref _hasPro, value);
        }

        bool _hardRefresh = true;

        public PayeesPageViewModel(Lazy<IResourceContainer> resourceContainer,
                                   INavigationService navigationService,
                                   IPageDialogService dialogService,
                                   Lazy<IPayeeLogic> payeeLogic,
                                   Lazy<ISyncFactory> syncFactory,
                                   Lazy<IPurchaseService> purchaseService,
                                   IEventAggregator eventAggregator)
        {
            _resourceContainer = resourceContainer;
            _payeeLogic = payeeLogic;
            _navigationService = navigationService;
            _dialogService = dialogService;
            _syncFactory = syncFactory;
            _purchaseService = purchaseService;
            _eventAggregator = eventAggregator;

            Payees = new ObservableList<Payee>();
            SelectedPayee = null;

            SelectedCommand = new DelegateCommand<Payee>(async p => await ExecuteSelectedCommand(p));
            RefreshCommand = new DelegateCommand(async () => await ExecuteRefreshCommand());
            SaveCommand = new DelegateCommand<Payee>(async p => await ExecuteSaveCommand(p));
            SaveSearchCommand = new DelegateCommand(async () => await ExecuteSaveSearchCommand());
            AddCommand = new DelegateCommand(async () => await ExecuteAddCommand());
            EditCommand = new DelegateCommand<Payee>(async a => await ExecuteEditCommand(a));
            AddTransactionCommand = new DelegateCommand(async () => await ExecuteAddTransactionCommand());
            RefreshPayeeCommand = new DelegateCommand<Payee>(ExecuteRefreshPayeeCommand);

            _eventAggregator.GetEvent<PayeeSavedEvent>().Subscribe(ExecuteRefreshPayeeCommand);
            _eventAggregator.GetEvent<PayeeDeletedEvent>().Subscribe(ExecuteRefreshPayeeCommand);
            _eventAggregator.GetEvent<PayeeHiddenEvent>().Subscribe(ExecuteRefreshPayeeCommand);
            _eventAggregator.GetEvent<PayeeUnhiddenEvent>().Subscribe(ExecuteRefreshPayeeCommand);
            _eventAggregator.GetEvent<TransactionSavedEvent>().Subscribe(async t => await RefreshPayeeFromTransaction(t));
        }

        public override async void OnActivated()
        {
            var purchasedPro = await _purchaseService.Value.VerifyPurchaseAsync(Purchases.Pro);
            HasPro = purchasedPro.Success;

            if (_hardRefresh)
            {
                await ExecuteRefreshCommand();
                _hardRefresh = false;
            }
        }

        public override async void OnDeactivated()
        {
            SelectedPayee = null;

            if (_needToSync)
            {
                var syncService = _syncFactory.Value.GetSyncService();
                var syncResult = await syncService.FullSync();

                if (syncResult.Success)
                {
                    await _syncFactory.Value.SetLastSyncDateTime(DateTime.Now);
                    _needToSync = false;
                }
            }
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        // this gets hit before the OnActivated
        public void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.TryGetValue(PageParameter.Payee, out Payee payee))
            {
                ExecuteRefreshPayeeCommand(payee);
            }

            if (parameters.TryGetValue(PageParameter.Transaction, out Transaction transaction))
            {
                ExecuteRefreshPayeeCommand(transaction.Payee);
            }
        }

        public async Task ExecuteSelectedCommand(Payee payee)
        {
            if (payee == null)
            {
                return;
            }

            if (payee.IsGenericHiddenPayee)
            {
                _hardRefresh = true;
                await _navigationService.NavigateAsync(PageName.HiddenPayeesPage);
            }
            else
            {
                var parameters = new NavigationParameters
                {
                    { PageParameter.Payee, payee }
                };

                await _navigationService.NavigateAsync(PageName.PayeeInfoPage, parameters);
            }
        }

        public async Task ExecuteRefreshCommand()
        {
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;

            try
            {
                var result = await _payeeLogic.Value.GetPayeesAsync();

                if (result.Success)
                {
                    Payees.ReplaceRange(result.Data);
                }
                else
                {
                    await _dialogService.DisplayAlertAsync(_resourceContainer.Value.GetResourceString("AlertRefreshUnsuccessful"), result.Message, _resourceContainer.Value.GetResourceString("AlertOk"));
                }

                NoPayees = (Payees?.Count ?? 0) == 0;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void ExecuteRefreshPayeeCommand(Payee payee)
        {
            var payees = Payees.Where(a => a.Id != payee.Id).ToList();

            if (payee != null && payee.IsActive)
            {
                payees.Add(payee);
            }

            Payees.ReplaceRange(payees);
        }

        public async Task ExecuteSaveCommand(Payee payee)
        {
            var result = await _payeeLogic.Value.SavePayeeAsync(payee);

            if (result.Success)
            {
                _eventAggregator.GetEvent<PayeeSavedEvent>().Publish(result.Data);
                _needToSync = true;
            }
            else
            {
                await _dialogService.DisplayAlertAsync(_resourceContainer.Value.GetResourceString("AlertSaveUnsuccessful"), result.Message, _resourceContainer.Value.GetResourceString("AlertOk"));
            }
        }

        public async Task ExecuteSaveSearchCommand()
        {
            var newPayee = new Payee
            {
                Description = SearchText
            };

            var result = await _payeeLogic.Value.SavePayeeAsync(newPayee);

            if (result.Success)
            {
                _eventAggregator.GetEvent<PayeeSavedEvent>().Publish(result.Data);
                _needToSync = true;
            }
            else
            {
                await _dialogService.DisplayAlertAsync(_resourceContainer.Value.GetResourceString("AlertSaveUnsuccessful"), result.Message, _resourceContainer.Value.GetResourceString("AlertOk"));
            }
        }

        public async Task ExecuteAddCommand()
        {
            await _navigationService.NavigateAsync(PageName.PayeeEditPage);

            SelectedPayee = null;
        }

        public async Task ExecuteEditCommand(Payee payee)
        {
            if (!payee.IsGenericHiddenPayee)
            {
                var parameters = new NavigationParameters
                {
                    { PageParameter.Payee, payee }
                };
                await _navigationService.NavigateAsync(PageName.PayeeEditPage, parameters);
            }
        }

        public async Task ExecuteAddTransactionCommand()
        {
            await _navigationService.NavigateAsync(PageName.TransactionEditPage);
        }

        async Task RefreshPayeeFromTransaction(Transaction transaction)
        {
            if (transaction != null && transaction.Payee != null)
            {
                var updatedPayeeResult = await _payeeLogic.Value.GetPayeeAsync(transaction.Payee.Id);
                if (updatedPayeeResult.Success)
                {
                    ExecuteRefreshPayeeCommand(updatedPayeeResult.Data);
                }
            }
        }
    }
}

