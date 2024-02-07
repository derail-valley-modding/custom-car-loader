﻿using CCL.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CCL.Creator.Validators
{
    internal interface ICarValidator : IComparable<ICarValidator>
    {
        string TestName { get; }
        ValidationResult Validate(CustomCarType car);
    }

    /// <summary>
    /// Inherit from this class to run validation on an entire <see cref="CustomCarType"/>.
    /// Use <see cref="RequiresStepAttribute"/> to specify ordering.
    /// </summary>
    [InheritedExport(typeof(ICarValidator))]
    internal abstract class CarValidator : ICarValidator
    {
        public abstract string TestName { get; }

        #region IComparable Implementation

        public int CompareTo(ICarValidator other)
        {
            Type thisType = GetType();
            Type otherType = other.GetType();

            var reqs = GetRequirements(GetType());
            var otherReqs = GetRequirements(otherType);

            if (ContainsRequirement(reqs, otherType))
            {
                return 1;
            }
            else if (ContainsRequirement(otherReqs, thisType))
            {
                return -1;
            }

            return reqs.Count.CompareTo(otherReqs.Count);
        }

        private static bool ContainsRequirement(IEnumerable<Type> reqList, Type potentialReq)
        {
            return reqList.Any(t => t.IsAssignableFrom(potentialReq));
        }

        private static List<Type> GetRequirements(Type type)
        {
            var result = new List<Type>();

            foreach (var attr in type.GetCustomAttributes<RequiresStepAttribute>())
            {
                result.Add(attr.RequiredStepType);
                result.AddRange(GetRequirements(attr.RequiredStepType));
            }

            return result;
        }

        #endregion

        /// <summary>Implement your car type validation logic in this method</summary>
        public abstract ValidationResult Validate(CustomCarType car);

        /// <summary>Create a new result with a single failure entry</summary>
        protected ValidationResult Fail(string message)
        {
            var result = new ValidationResult(TestName);
            result.Fail(message);
            return result;
        }

        /// <summary>Create a new result with a single critical failure (critical failures stop validation)</summary>
        protected ValidationResult CriticalFail(string message)
        {
            var result = new ValidationResult(TestName);
            result.CriticalFail(message);
            return result;
        }

        /// <summary>Create a new default result with passing status</summary>
        protected ValidationResult Pass() => new ValidationResult(TestName);

        /// <summary>Create a new result with skipped status (should be returned without modification)</summary>
        protected ValidationResult Skip()
        {
            var result = new ValidationResult(TestName);
            result.Skip();
            return result;
        }
    }

    /// <summary>Inherit from this class to run validation on each <see cref="CustomCarVariant"/> belonging to a car type</summary>
    internal abstract class LiveryValidator : CarValidator
    {
        public override ValidationResult Validate(CustomCarType car)
        {
            var overallResult = new ValidationResult(TestName);

            foreach (var livery in car.liveries)
            {
                var curResult = ValidateLivery(livery);
                overallResult.Merge(curResult);
            }

            return overallResult;
        }

        /// <summary>Implement your livery validation logic in this method</summary>
        protected abstract ValidationResult ValidateLivery(CustomCarVariant livery);
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    internal class RequiresStepAttribute : Attribute
    {
        public readonly Type RequiredStepType;

        public RequiresStepAttribute(Type requiredStepType)
        {
            if (!typeof(CarValidator).IsAssignableFrom(requiredStepType))
            {
                throw new ArgumentOutOfRangeException(nameof(requiredStepType), "Requirement must be a CarValidator");
            }
            RequiredStepType = requiredStepType;
        }
    }

    internal enum ResultStatus
    {
        Pass, Skipped, Warning, Failed, Critical
    }

    /// <summary>Used to bundle the validation status and any warning messages from a particular validator</summary>
    internal class ValidationResult
    {
        public readonly string TestName;
        public ResultStatus Status { get; private set; } = ResultStatus.Pass;

        private readonly List<ResultEntry> _warnings = new List<ResultEntry>();
        public IEnumerable<ResultEntry> Warnings => _warnings;
        public IEnumerable<ResultEntry> Entries => 
            (_warnings.Count > 0) ?
            _warnings :
            (IEnumerable <ResultEntry>)new[] { new ResultEntry(TestName, Status) };

        /// <summary>Whether this result requires/suggests correction by the user</summary>
        public bool IsCorrectable => _warnings.Any();

        /// <summary>Whether this result prevent exporting the car</summary>
        public bool AnyFailure => _warnings.Any(e => e.IsFailure);

        /// <summary>Create a default result - the initial status is 0 errors and warnings (passing)</summary>
        public ValidationResult(string testName)
        {
            TestName = testName;
        }

        private void Escalate(ResultStatus latest)
        {
            if (latest > Status)
            {
                Status = latest;
            }
        }

        /// <summary>Add a warning message to this result. If status is currently Passing, this will escalate it to Warning level</summary>
        public void Warning(string message)
        {
            _warnings.Add(new ResultEntry(TestName, ResultStatus.Warning, message));
            Escalate(ResultStatus.Warning);
        }

        /// <summary>Add a failure message to this result. If status is currently Warning or below, this will escalate it to Failure level</summary>
        public void Fail(string message)
        {
            _warnings.Add(new ResultEntry(TestName, ResultStatus.Failed, message));
            Escalate(ResultStatus.Failed);
        }

        /// <summary>Add a critical failure message to this result. Status will be escalated to Critical and no more validators will be run</summary>
        public void CriticalFail(string message)
        {
            _warnings.Add(new ResultEntry(TestName, ResultStatus.Critical, message));
            Escalate(ResultStatus.Critical);
        }

        /// <summary>Clear all warnings and set the status to Skipped</summary>
        public void Skip()
        {
            _warnings.Clear();
            Status = ResultStatus.Skipped;
        }

        /// <summary>Copy all warnings from another result into this one. Status will be raised to the maximum between the two results</summary>
        public void Merge(ValidationResult other)
        {
            Escalate(other.Status);
            _warnings.AddRange(other.Warnings);
        }
    }

    internal class ResultEntry
    {
        public string TestName;
        public ResultStatus Status;
        public string Message;

        public ResultEntry(string testName, ResultStatus status, string message = "")
        {
            TestName = testName;
            Status = status;
            Message = message;
        }

        private static readonly ResultStatus[] correctable =
        {
            ResultStatus.Warning, ResultStatus.Failed, ResultStatus.Critical
        };

        public bool IsCorrectable => correctable.Contains(Status);
        public bool IsFailure => Status == ResultStatus.Failed || Status == ResultStatus.Critical;

        public Color StatusColor
        {
            get
            {
                return Status switch
                {
                    ResultStatus.Pass => Color.green,
                    ResultStatus.Warning => Color.yellow,
                    ResultStatus.Failed => Color.red,
                    ResultStatus.Critical => Color.red,
                    _ => Color.black,
                };
            }
        }
    }
}
