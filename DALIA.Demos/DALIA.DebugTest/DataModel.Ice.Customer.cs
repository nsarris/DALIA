using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.ICE
{
    [Table(Name = "ICESRCPF", Schema = "XCRS")]
    public class Customer
    {
        [Column(IsPrimaryKey = true, Name = "SEARCHID", DataType = LinqToDB.DataType.Decimal)]
        public decimal SearchId { get; set; }

        [Column(Name = "SERSEQ", DataType = LinqToDB.DataType.Decimal)]
        public decimal SearchSequence { get; set; }

        [Column(Name = "RELATION", DataType = LinqToDB.DataType.NVarChar)]
        public string Relation { get; set; }

        /// <summary>
        /// ??????
        /// </summary>
        [Column(Name = "IPIDCRA", DataType = LinqToDB.DataType.Decimal)]
        public decimal CrsId { get; set; }

        [Column(Name = "TAXID", DataType = LinqToDB.DataType.NVarChar)]
        public string TaxId { get; set; }

        [Column(Name = "DOYCODE", DataType = LinqToDB.DataType.NVarChar)]
        public string DoyCode { get; set; }

        [Column(Name = "BRANCH_ID", DataType = LinqToDB.DataType.NVarChar)]
        public string Branchid { get; set; }

        [Column(Name = "IP_TYPE", DataType = LinqToDB.DataType.NVarChar)]
        public string IpType { get; set; }

        [Column(Name = "CSTCTG", DataType = LinqToDB.DataType.NVarChar)]
        public string CSTCTG { get; set; }

        [Column(Name = "CSTLSTNM", DataType = LinqToDB.DataType.NVarChar)]
        public string LastName { get; set; }

        [Column(Name = "CSTFSTNM", DataType = LinqToDB.DataType.NVarChar)]
        public string FirstName { get; set; }

        [Column(Name = "CSTFTHNM", DataType = LinqToDB.DataType.NVarChar)]
        public string FathersName { get; set; }

        [Column(Name = "CSTBTHDT", DataType = LinqToDB.DataType.NVarChar)]
        public string CSTBTHDT { get; set; }

        [Column(Name = "MAIN_ID", DataType = LinqToDB.DataType.NVarChar)]
        public string MainId { get; set; }

        [Column(Name = "MAINADDR", DataType = LinqToDB.DataType.NVarChar)]
        public string MainAddress { get; set; }

        [Column(Name = "ACTYEARS", DataType = LinqToDB.DataType.NVarChar)]
        public string ActYears { get; set; }

        [Column(Name = "CSTOCPCD", DataType = LinqToDB.DataType.NVarChar)]
        public string CSTOCPCD { get; set; }

        [Column(Name = "LSTCHGDT", DataType = LinqToDB.DataType.DateTime)]
        public string LSTCHGDT { get; set; }

        [Column(Name = "EMPLCODE", DataType = LinqToDB.DataType.NVarChar)]
        public string EMPLCODE { get; set; }

        [Column(Name = "EMPLPOS", DataType = LinqToDB.DataType.NVarChar)]
        public string EMPLPOS { get; set; }

        [Column(Name = "EMPLNME", DataType = LinqToDB.DataType.NVarChar)]
        public string EMPLNME { get; set; }
    }
}
