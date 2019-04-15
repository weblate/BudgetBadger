﻿using System;
using System.Collections.Generic;
using System.Linq;
using BudgetBadger.Models.Interfaces;

namespace BudgetBadger.Models
{
    public class Payee : BaseModel, IValidatable, IDeepCopy<Payee>, IEquatable<Payee>, IPropertyCopy<Payee>, IComparable
    {
        Guid id;
        public Guid Id
        {
            get => id;
            set => SetProperty(ref id, value);
        }

        string description;
        public string Description
        {
            get => description;
            set { SetProperty(ref description, value); OnPropertyChanged(nameof(Group)); }
            }

        string notes;
        public string Notes
        {
            get => notes;
            set => SetProperty(ref notes, value);
        }

        //calculated
        bool isAccount;
        public bool IsAccount
        {
            get => isAccount;
            set { SetProperty(ref isAccount, value); OnPropertyChanged(nameof(Group)); }
        }

        DateTime? createdDateTime;
        public DateTime? CreatedDateTime
        {
            get => createdDateTime;
            set { SetProperty(ref createdDateTime, value); OnPropertyChanged(nameof(IsNew)); OnPropertyChanged(nameof(IsActive)); }
        }

        public bool IsNew { get => CreatedDateTime == null; }

        DateTime? modifiedDateTime;
        public DateTime? ModifiedDateTime
        {
            get => modifiedDateTime;
            set => SetProperty(ref modifiedDateTime, value);
        }

        DateTime? deletedDateTime;
        public DateTime? DeletedDateTime
        {
            get => deletedDateTime;
            set { SetProperty(ref deletedDateTime, value); OnPropertyChanged(nameof(IsDeleted)); OnPropertyChanged(nameof(IsActive)); }
        }

        public bool IsDeleted { get => DeletedDateTime != null; }

        public bool IsActive { get => !IsNew && !IsDeleted; }

        public bool IsStartingBalance
        {
            get => Id == Constants.StartingBalancePayee.Id;
        }

        public string Group
        {
            get
            {
                if (string.IsNullOrEmpty(Description))
                {
                    return "";
                }
                else if (IsAccount)
                {
                    return "Transfer";
                }
                else
                {
                    return Description[0].ToString().ToUpper();
                }
            }
        }

        public Payee()
        {
            Id = Guid.Empty;
        }

        public Payee DeepCopy()
        {
            return (Payee)this.MemberwiseClone();
        }

        public void PropertyCopy(Payee item)
        {
            Description = item.description;
            Notes = item.Notes;
            IsAccount = item.IsAccount;
            CreatedDateTime = item.CreatedDateTime;
            ModifiedDateTime = item.ModifiedDateTime;
            DeletedDateTime = item.DeletedDateTime;
        }

        public Result Validate()
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(Description))
            {
                errors.Add("Payee description is required");
            }

            return new Result { Success = !errors.Any(), Message = string.Join(Environment.NewLine, errors) };
        }

        public bool Equals(Payee p)
        {
            // If parameter is null, return false.
            if (p is null)
            {
                return false;
            }

            // Optimization for a common success case.
            if (Object.ReferenceEquals(this, p))
            {
                return true;
            }

            // If run-time types are not exactly the same, return false.
            if (this.GetType() != p.GetType())
            {
                return false;
            }

            // Return true if the fields match.
            // Note that the base class is not invoked because it is
            // System.Object, which defines Equals as reference equality.
            return Id == p.Id;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Payee);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public int CompareTo(object obj)
        {
            if (obj is null)
            {
                return 1;
            }

            var payee = (Payee)obj;

            if (IsAccount == payee.IsAccount)
            {
                if (Group == payee.Group)
                {
                    return String.Compare(Description, payee.Description);
                }

                return String.Compare(Group, payee.Group);
            }

            return -1 * IsAccount.CompareTo(payee.IsAccount);
        }

        public static bool operator ==(Payee lhs, Payee rhs)
        {
            // Check for null on left side.
            if (lhs is null)
            {
                if (rhs is null)
                {
                    // null == null = true.
                    return true;
                }

                // Only the left side is null.
                return false;
            }
            // Equals handles case of null on right side.
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Payee lhs, Payee rhs)
        {
            return !(lhs == rhs);
        }

        public static void PropertyCopy(Payee existing, Payee updated)
        {
            existing.PropertyCopy(updated);
        }
    }
}
