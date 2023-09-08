using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CCL.Creator.Validators
{
    internal class CarValidatorAttribute : Attribute
    {
        public readonly string TestName;

        public CarValidatorAttribute(string testName)
        {
            TestName = testName;
        }
    }

    internal enum ResultStatus
    {
        Pass, Warning, Failed, Skipped, Critical
    }

    internal class Result
    {
        public string? Name;
        public ResultStatus Status;
        public string Message;

        private Result(string? name, ResultStatus status, string message = "")
        {
            Name = name;
            Status = status;
            Message = message;
        }

        public static Result Pass() => new Result(null, ResultStatus.Pass);
        public static Result Warning(string message) => new Result(null, ResultStatus.Warning, message);
        public static Result Failed(string message) => new Result(null, ResultStatus.Failed, message);
        public static Result Critical(string message) => new Result(null, ResultStatus.Critical, message);
        public static Result Skipped() => new Result(null, ResultStatus.Skipped);

        private static readonly ResultStatus[] correctable =
        {
            ResultStatus.Warning, ResultStatus.Failed, ResultStatus.Critical
        };

        public bool IsCorrectable => correctable.Contains(Status);

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
