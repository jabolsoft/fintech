﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using FinTech.Core.Models;
using FinTech.Core.Interfaces;
using MongoDB.Bson;
using FinTech.Web.Helpers;
using System.Text;
using System.IO;

namespace FinTech.Web.Controllers
{
    [Route("api/transactions")]
    public class TransactionController : Controller
    {
        public ITransactionService TransactionService { get; set; }
        public TransactionController(ITransactionService transactionService)
        {
            TransactionService = transactionService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var transactions = TransactionService.GetTransactions();
            return new JsonResult(new { transactions }, JsonHelper.GetConfiguredOutputFormatter().SerializerSettings);
        }
        
        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            var transaction = TransactionService.GetTransactionById(id);
            return new JsonResult(new { transaction }, JsonHelper.GetConfiguredOutputFormatter().SerializerSettings);
        }
        
        [HttpPost]
        public void Create([FromBody]Transaction transaction)
        {
            TransactionService.CreateTransaction(transaction);
        }
        
        [HttpPut("{id}")]
        public void Update(string id, [FromBody]Transaction transaction)
        {
            TransactionService.UpdateTransaction(id, transaction);
        }
        
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            TransactionService.DeleteTransaction(id);
        }

        [HttpGet("export")]
        public IActionResult Export()
        {
            var tempFilename = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".qif");
            TransactionService.ExportTransactions(tempFilename);
            var exportBytes = System.IO.File.ReadAllBytes(tempFilename);
            var result = new FileContentResult(exportBytes, "application/qif");
            result.FileDownloadName = string.Format("Transactions-{0}.qif", DateTime.Now.ToShortDateString());
            return result;
        }
    }
}
