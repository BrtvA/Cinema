﻿using Cinema.BLL.DTOs.Request.Attributes;
using Cinema.BLL.DTOs.Request.Base;
using System.ComponentModel.DataAnnotations;

namespace Cinema.BLL.DTOs.Request;

public class HomeIndexReqDTO : DateReqDTO
{
    [ArrayMinValue(1)]
    public int[]? GenresId { get; set; }
    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;
}
