using Cinema.DAL.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace Cinema.BLL.DTOs.Request.Attributes;

internal class ArrayMinValueAttribute : ValidationAttribute
{
    public int Minimum { get; set; }
    public PositionModel MinimumPosition { get; set; } = null!;

    public ArrayMinValueAttribute(int minValue)
    {
        Minimum = minValue;
    }

    public ArrayMinValueAttribute(int minRow, int minColumn)
    {
        MinimumPosition = new PositionModel { 
            Row = minRow,
            Column = minColumn
        };
    }

    public override bool IsValid(object? value)
    {
        if (value is null)
        {
            return true;
        }

        if (value is int[] intArr)
        {
            foreach (var item in intArr)
            {
                if (item < Minimum)
                {
                    return false;
                }
            }
            return true;
        }
        else if (value is PositionModel[] positionArr)
        {
            foreach (var item in positionArr)
            {
                if (item.Row < MinimumPosition.Row 
                    || item.Column < MinimumPosition.Column)
                {
                    return false;
                }
            }
            return true;
        }

        return false;
    }
}
