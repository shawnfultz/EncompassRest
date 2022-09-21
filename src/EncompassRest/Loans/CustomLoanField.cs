using System;

namespace EncompassRest.Loans
{
    internal sealed class CustomLoanField : LoanField
    {
        public override object? Value
        {
            get
            {
                var customField = Loan.CustomFields.GetById(FieldId);
                if (customField != null)
                {
                    if (Descriptor is NonStandardFieldDescriptor && (Descriptor?.Format == Schema.LoanFieldFormat.STRING))
                    {
                        //If it is a string, just return the string
                        return customField.StringValue;
                    }
                    else
                    if (Descriptor is NonStandardFieldDescriptor && (Descriptor?.Format == Schema.LoanFieldFormat.INTEGER))
                    {
                        //If it is an integer, don't return a double (NumericValue is a double)
                        return Convert.ToInt32(customField.NumericValue);
                    }
                    else
                    {
                        if (customField.DateValue.HasValue)
                        {
                            return customField.DateValue;
                        }
                        if (customField.NumericValue.HasValue)
                        {
                            return customField.NumericValue;
                        }
                        return customField.StringValue;
                    }
                }
                return null;
            }
            set
            {
                if (value is LoanField loanField)
                {
                    value = loanField.Value;
                }

                var customField = Loan.CustomFields.GetById(FieldId);
                if (customField == null)
                {
                    customField = new CustomField { FieldName = FieldId };
                    Loan.CustomFields.Add(customField);
                }

                //Dates, numeric values, and strings should all be set as a stringValue without regard to the custom field definition.
                //Encompass doesn't work with setting a dateValue for a DATE or DATETIME custom field or numericValue for an INTEGER or DECIMAL_2 custom field, it will just null out the value if you try.
                customField.StringValue = value?.ToString();

                /*
                if (customField.DateValue.HasValue || customField._dateValue?.Dirty == true)
                {
                    customField.DateValue = value != null ? Convert.ToDateTime(value) : (DateTime?)null;
                    customField.StringValue = value?.ToString();
                    customField._stringValue!.Dirty = false;
                }
                else if (customField.NumericValue.HasValue || customField._numericValue?.Dirty == true)
                {
                    customField.NumericValue = value != null ? Convert.ToDecimal(value) : (decimal?)null;
                    customField.StringValue = value != null ? FormattedValue : null;
                    customField._stringValue!.Dirty = false;
                }
                else if (customField.StringValue != null || customField._stringValue?.Dirty == true)
                {
                    customField.StringValue = value?.ToString();
                }
                else
                {
                    switch (value)
                    {
                        case null:
                            customField.StringValue = null;
                            break;
                        case string str:
                            customField.StringValue = str;
                            break;
                        case DateTime dateTime:
                            customField.DateValue = dateTime;
                            break;
                        default:
                            customField.NumericValue = Convert.ToDecimal(value);
                            break;
                    }
                }
                */
            }
        }

        internal CustomLoanField(FieldDescriptor descriptor, Loan loan)
            : base(descriptor, loan)
        {
        }

        public override string? ToString()
        {
            var customField = Loan.CustomFields.GetById(FieldId);
            if (customField != null)
            {
                if (customField.StringValue != null)
                {
                    return customField.StringValue;
                }
                if (customField.DateValue.HasValue)
                {
                    return customField.DateValue.ToString();
                }
                if (customField.NumericValue.HasValue)
                {
                    return customField.NumericValue.ToString();
                }
            }
            return null;
        }
    }
}