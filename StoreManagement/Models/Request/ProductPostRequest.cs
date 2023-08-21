using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StoreManagement.Models;

public class ProductPostRequest
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid AuthorId { get; set; }
    public List<IFormFile> UploadFiles { get; set; }
    public string Content { get; set; }
    public string? CoverImg { get; set; }
}
