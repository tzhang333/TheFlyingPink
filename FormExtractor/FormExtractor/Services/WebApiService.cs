using FormExtractor.Models;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;
using Vendor = FormExtractor.Models.Vendor;

namespace FormExtractor.Services
{
    public class WebApiService
    {
        RestClient _client;
        string _baseUrl;



        public WebApiService()
        {
            _baseUrl = ConfigurationManager.AppSettings["Sage300WebApiUrl"];
            var user = ConfigurationManager.AppSettings["WebApiUser"];
            var pass = ConfigurationManager.AppSettings["WebApiPass"];
            _client = new RestClient(_baseUrl);
            _client.Authenticator = new HttpBasicAuthenticator(user, pass);
        }

        private static Vendor MapVendor(SageVendor sageVendor)
        {
            return new Vendor()
            {
                Id = sageVendor.VendorNumber,
                Name = sageVendor.VendorName
            };
        }

        private static SageVendor MapSageVendor(Vendor vendor)
        {
            return new SageVendor()
            {
                VendorNumber = vendor.Id,
                VendorName = vendor.Name
            };
        }

        public List<Vendor> GetVendors(string company)
        {
            var list = new List<Vendor>();

            var url = string.Format("{0}{1}/AP/APVendors", _baseUrl, company);
            var request = new RestRequest(url, Method.GET);

            var response = _client.Get<dynamic>(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var data = (Dictionary<string, object>)response.Data;
                object sdata = null;
                data.TryGetValue("value", out sdata);

                var serializer = new JsonSerializer();
                var ldata = JsonConvert.DeserializeObject<List<SageVendor>>(sdata.ToString());

                foreach (var vendor in ldata)
                {
                    list.Add(MapVendor(vendor));
                }
            }

            return list;
        }

        public Vendor GetVendor(string company, string number)
        {
            var list = new List<Vendor>();

            var url = string.Format("{0}{1}/AP/APVendors" + "('" + number + "')", _baseUrl, company);
            var request = new RestRequest(url, Method.GET);

            var response = _client.Get<dynamic>(request);

            var ret = new Vendor();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                ret = (Vendor)response.Data;
            }

            return ret;
        }

        /// <summary>
        /// jjj
        /// </summary>
        /// <param name="company"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool hahaha(string company)
        {
            var url = string.Format("{0}{1}/AP/APInvoiceBatches", _baseUrl, company);
            var request = new RestRequest(url, Method.POST);

            var ppp = @"{
                'BatchNumber': 0,
  'BatchDate': '2019-06-13T00:00:00Z',
  'Description': '',
  'NumberOfEntries': 1,
  'BatchTotal': 12.99,
  'BatchType': 'Entered',
  'BatchStatus': 'Open',
  'InvoiceType': 'Summary',
  'LastEntryNumber': 1,
  'PostingSequenceNumber': 0,
  'NumberOfErrors': 0,
  'DateLastEdited': '2019-06-14T00:00:00Z',
  'BatchPrintedFlag': 'No',
  'SourceApplication': 'AP',
  'ICTRelated': 'No',
  'ProcessCommandCode': 'UnlockBatchResource',
  'Invoices': [
    {
      'BatchNumber': 0,
      'EntryNumber': 0,
      'Originator': '',
      'VendorNumber': '1200',
      'DocumentNumber': 'TESTING22222',
      'RemitToLocation': '',
      'DocumentType': 'Invoice',
      'TransactionType': 'InvoiceSummaryEntered',
      'OrderNumber': '',
      'PONumber': '',
      'InvoiceDescription': '',
      'ApplytoDocument': '',
      'AccountSet': 'USA',
      'DocumentDate': '2019-06-13T00:00:00Z',
      'AsOfDate': '2019-06-13T00:00:00Z',
      'FiscalYear': '2019',
      'FiscalPeriod': '06',
      'CurrencyCode': 'USD',
      'RateType': 'SP',
      'RateOverridden': 'No',
      'ExchangeRate': 1,
      'ApplytoExchangeRate': 0,
      'Terms': 'DUETBL',
      'TermsOverridden': 'No',
      'DueDate': '2019-06-25T00:00:00Z',
      'DiscountDate': null,
      'DiscountPercentage': 0,
      'DiscountAmountAvailable': 0,
      'NumberOfDetailsDecimal': 1,
      'Taxable': 'No',
      'TaxAmountControl': 'Calculate',
      'TaxGroup': 'USDTAX',
      'TaxAuthority1': 'STATE',
      'TaxAuthority2': 'COUNTY',
      'TaxAuthority3': '',
      'TaxAuthority4': '',
      'TaxAuthority5': '',
      'TaxClass1': 1,
      'TaxClass2': 1,
      'TaxClass3': 0,
      'TaxClass4': 0,
      'TaxClass5': 0,
      'TaxBase1': 12,
      'TaxBase2': 12,
      'TaxBase3': 0,
      'TaxBase4': 0,
      'TaxBase5': 0,
      'TaxAmount1': 0.75,
      'TaxAmount2': 0.24,
      'TaxAmount3': 0,
      'TaxAmount4': 0,
      'TaxAmount5': 0,
      'Num1099CPRSAmount': 0,
      'DistributionSetAmount': 0,
      'TotalDistributedTax': 0.99,
      'DocumentTotalBeforeTaxes': -0.99,
      'DistributedAllocatedTaxes': 0,
      'NumberOfScheduledPayments': 1,
      'DistributedTotalBeforeTaxes': 12,
      'DistributedTotalIncludingTax': 12.99,
      'PrepaymentNumber': '',
      'LocationName': '',
      'AddressLine1': '927 Warehouse Road',
      'AddressLine2': '',
      'AddressLine3': '',
      'AddressLine4': '',
      'City': 'Kansas City',
      'StateProvince': 'MO',
      'ZipPostalCode': '59782',
      'Country': 'USA',
      'ContactName': 'Mr. Carl Jenner',
      'PhoneNumber': '8165553341',
      'FaxNumber': '8165553340',
      'RateDate': '2019-06-13T00:00:00Z',
      'RecoverableTaxes': 0,
      'VendorGroupCode': 'INV',
      'TermsDescription': 'Due by Invoice Date',
      'DistributionSet': '',
      'Num1099CPRSCode': '3',
      'AmountDue': 0,
      'GeneratePaymentSchedule': 0,
      'TaxTotal': 0.99,
      'DocumentTotalIncludingTax': 0,
      'UndistributedAmount': -12.99,
      'TaxInclusive1': 'No',
      'TaxInclusive2': 'No',
      'TaxInclusive3': 'No',
      'TaxInclusive4': 'No',
      'TaxInclusive5': 'No',
      'ExpensedSeparatelyTaxes': 0,
      'TaxAmountTobeAllocated': 0.99,
      'CurrencyCodeOperator': 'Multiply',
      'RecoverableAccount1': '',
      'RecoverableAccount2': '',
      'RecoverableAccount3': '',
      'RecoverableAccount4': '',
      'RecoverableAccount5': '',
      'ExpenseSeparatelyAccount1': '',
      'ExpenseSeparatelyAccount2': '',
      'ExpenseSeparatelyAccount3': '',
      'ExpenseSeparatelyAccount4': '',
      'ExpenseSeparatelyAccount5': '',
      'DrillDownApplicationSource': '',
      'DrillDownType': 0,
      'DrillDownLinkNumber': 0,
      'PropertyCode': 'ModeNormal1Batch2',
      'PropertyValue': 1,
      'ProcessCommandCode': 'DistributeTaxes',
      'JobRelated': 'No',
      'DistributionRecoverableTaxes': 0,
      'DistributionExpenseSeparatelyTaxes': 0,
      'ErrorBatch': 0,
      'ErrorEntry': 0,
      'Email': 'customer_service@chlorideus.com',
      'ContactsPhone': '',
      'ContactsFax': '',
      'ContactsEmail': 'ckjenner@chlorideus.com',
      'PrepaymentAmount': 0,
      'RecurringPayableCode': '',
      'DateGenerated': null,
      'DiscountBaseWithTax': 12.99,
      'DiscountBaseWithoutTax': 12,
      'DiscountBase': 12,
      'RetainageInvoice': 'No',
      'HasRetainage': 'No',
      'OriginalDocumentNumber': '',
      'RetainageAmount': 0,
      'PercentRetained': 0,
      'DaysRetained': 0,
      'RetainageDueDate': null,
      'RetainageTermsCode': '',
      'RetainageDueDateOverride': 'No',
      'RetainageAmountOverride': 'No',
      'RetainageExchangeRate': 'UseOriginalDocumentExchangeRate',
      'TaxBaseControl': 'Calculate',
      'NumberOfOptionalFields': 6,
      'NumberOfDetails': 1,
      'SourceApplication': 'AP',
      'OnHold': 'No',
      'OrigExists': 1,
      'OrigName': '',
      'OrigStatus': 1,
      'VendorExists': 1,
      'VendorName': 'Chloride Systems',
      'VendorDistType': 1,
      'VendorDistCode': 'INV',
      'VendorAccount': '',
      'VendorTaxReportType': 1,
      'RemitExists': 0,
      'RemitName': '',
      'Num1099CPRSExists': 1,
      'Num1099CPRSDescription': 'Other Income',
      'DistSetExists': 0,
      'DistSetDescription': '',
      'DistSetMethod': 1,
      'TermExists': 1,
      'TermUsePaymentSchedule': 0,
      'RTGTermExists': 0,
      'RTGTermDescription': '',
      'PMLevel1Name': 'Contract',
      'PMLevel2Name': 'Project',
      'PMLevel3Name': 'Category',
      'TaxGroupDescription': 'US Sales Tax - Purchase',
      'TaxAuth1Description': 'State Tax',
      'TaxAuth2Description': 'County Tax',
      'TaxAuth3Description': '',
      'TaxAuth4Description': '',
      'TaxAuth5Description': '',
      'APVersionCreatedIn': '67A',
      'TaxStateVersion': 1,
      'ReportRetainageTax': 0,
      'TaxReportingCurrencyCode': 'USD',
      'TaxReportingCalculateMethod': 'Calculate',
      'TaxReportingExchangeRate': 1,
      'TaxReportingRateType': '',
      'TaxReportingRateDate': null,
      'TaxReportingRateOperator': 'Multiply',
      'TaxReportingRateOverride': 'No',
      'TaxReportingAmount1': 0.75,
      'TaxReportingAmount2': 0.24,
      'TaxReportingAmount3': 0,
      'TaxReportingAmount4': 0,
      'TaxReportingAmount5': 0,
      'TaxReportingTotal': 0.99,
      'TaxReportingAllocatedTax': 0.99,
      'TaxReportingExpensedTax': 0,
      'TaxReportingRecoverableTax': 0,
      'RetainageTaxBase1': 0,
      'RetainageTaxBase2': 0,
      'RetainageTaxBase3': 0,
      'RetainageTaxBase4': 0,
      'RetainageTaxBase5': 0,
      'RetainageTaxAmount1': 0,
      'RetainageTaxAmount2': 0,
      'RetainageTaxAmount3': 0,
      'RetainageTaxAmount4': 0,
      'RetainageTaxAmount5': 0,
      'FunctionalTaxBase1': 12,
      'FunctionalTaxBase2': 12,
      'FunctionalTaxBase3': 0,
      'FunctionalTaxBase4': 0,
      'FunctionalTaxBase5': 0,
      'FunctionalTaxAmount1': 0.75,
      'FunctionalTaxAmount2': 0.24,
      'FunctionalTaxAmount3': 0,
      'FunctionalTaxAmount4': 0,
      'FunctionalTaxAmount5': 0,
      'FunctionalDistributionWithTaxTotal': 12.99,
      'FunctionalRetainageAmount': 0,
      'FunctionalDiscountAmount': 0,
      'Functional1099CPRSAmount': 0,
      'FunctionalPrepaymentAmount': 0,
      'SourceAmountDue': 12.99,
      'FunctionalAmountDue': 12.99,
      'VendorName2': 'Chloride Systems',
      'RetainageTaxTotal': 0,
      'TaxAmount1Total': 0.75,
      'TaxAmount2Total': 0.24,
      'TaxAmount3Total': 0,
      'TaxAmount4Total': 0,
      'TaxAmount5Total': 0,
      'RetainageAmountFromDetails': 0,
      'EnteredBy': 'ADMIN',
      'PostingDate': '2019-06-13T00:00:00Z',
      'AcctSetDescription': 'Accounts payable, Other',
      'ImportDeclarationNumber': '',
      'EstimatedTaxWithheldAmount1': 0,
      'EstimatedTaxWithheldAmount2': 0,
      'EstimatedTaxWithheldAmount3': 0,
      'EstimatedTaxWithheldAmount4': 0,
      'EstimatedTaxWithheldAmount5': 0,
      'ReverseChargesBase1': 0,
      'ReverseChargesBase2': 0,
      'ReverseChargesBase3': 0,
      'ReverseChargesBase4': 0,
      'ReverseChargesBase5': 0,
      'ReverseChargesAmount1': 0,
      'ReverseChargesAmount2': 0,
      'ReverseChargesAmount3': 0,
      'ReverseChargesAmount4': 0,
      'ReverseChargesAmount5': 0,
      'NegativeReverseChargesAmount': 0,
      'NetVendorTaxAmount': 0.99,
      'ReverseChargesAmount': 0,
      'TotalTaxWithholding': 0,
      'AmountDuewithTaxWithholding': 0,
      'AmtDueDiscWithholdAdjusted': 0,
      'InvoiceDetails': [
        {
          'BatchNumber': 28,
          'EntryNumber': 1,
          'LineNumber': 20,
          'Destination': '',
          'RouteNumber': 0,
          'DistributionCode': 'INV',
          'DistributionDescription': 'Purchase of inventory',
          'TaxTotal': 0.99,
          'ManualTaxEntry': 'No',
          'BaseTax1': 12,
          'BaseTax2': 12,
          'BaseTax3': 0,
          'BaseTax4': 0,
          'BaseTax5': 0,
          'TaxClass1': 1,
          'TaxClass2': 1,
          'TaxClass3': 0,
          'TaxClass4': 0,
          'TaxClass5': 0,
          'TaxInclusive1': 'No',
          'TaxInclusive2': 'No',
          'TaxInclusive3': 'No',
          'TaxInclusive4': 'No',
          'TaxInclusive5': 'No',
          'TaxRate1': 6.25,
          'TaxRate2': 2,
          'TaxRate3': 0,
          'TaxRate4': 0,
          'TaxRate5': 0,
          'TaxAmount1': 0.75,
          'TaxAmount2': 0.24,
          'TaxAmount3': 0,
          'TaxAmount4': 0,
          'TaxAmount5': 0,
          'GLAccount': '1300',
          'RetainageAllocatedTaxAccount': '1300',
          'DistributedAmount': 12,
          'Comment': '',
          'DistributedAmountBeforeTaxes': 12,
          'TaxAmountIncludedInPrice': 0,
          'GLDistributedAmount': 12.99,
          'RecoverableTaxAmount1': 0,
          'RecoverableTaxAmount2': 0,
          'RecoverableTaxAmount3': 0,
          'RecoverableTaxAmount4': 0,
          'RecoverableTaxAmount5': 0,
          'ExpenseSeparatelyTaxAmount1': 0,
          'ExpenseSeparatelyTaxAmount2': 0,
          'ExpenseSeparatelyTaxAmount3': 0,
          'ExpenseSeparatelyTaxAmount4': 0,
          'ExpenseSeparatelyTaxAmount5': 0,
          'TaxAllocatedTotal': 0.99,
          'Contract': '',
          'Project': '',
          'Category': '',
          'ProjectCategoryResource': '',
          'TransactionNumber': 0,
          'CostClass': 'None',
          'BillingType': 'None',
          'ItemNumber': '',
          'UnitOfMeasure': '',
          'Quantity': 0,
          'Cost': 0,
          'BillingDate': '2019-06-13T00:00:00Z',
          'BillingRate': 0,
          'BillingCurrency': '',
          'CommentAttached': 'No',
          'Discountable': 'Yes',
          'OriginalLineIdentifier': 0,
          'RetainageAmount': 0,
          'PercentRetained': 0,
          'DaysRetained': 0,
          'RetainageDueDate': null,
          'RetainageDueDateOverride': 'No',
          'RetainageAmountOverride': 'No',
          'NumberOfOptionalFields': 5,
          'ProcessCommandCode': 'InsertOptionalFields',
          'DestinationExists': 1,
          'DestinationDescription': '',
          'DestinationStatus': 1,
          'DistributionCodeExists': 1,
          'DistributionCodeDescription': 'Purchase of inventory',
          'DistributionCodeStatus': 1,
          'RouteExists': 0,
          'RouteDescription': '',
          'RouteStatus': 1,
          'AccountExists': 1,
          'AccountDescription': 'Inventory',
          'AccountStatus': 1,
          'RetainageDistributionAmount': 0,
          'InvoicedRetainageDistribution': 0,
          'TaxReportingAmount1': 0.75,
          'TaxReportingAmount2': 0.24,
          'TaxReportingAmount3': 0,
          'TaxReportingAmount4': 0,
          'TaxReportingAmount5': 0,
          'TaxReportingTotal': 0.99,
          'TaxReportingAllocatedTax': 0.99,
          'TaxReportingExpensedTax1': 0,
          'TaxReportingExpensedTax2': 0,
          'TaxReportingExpensedTax3': 0,
          'TaxReportingExpensedTax4': 0,
          'TaxReportingExpensedTax5': 0,
          'TaxReportingRecoverableTax1': 0,
          'TaxReportingRecoverableTax2': 0,
          'TaxReportingRecoverableTax3': 0,
          'TaxReportingRecoverableTax4': 0,
          'TaxReportingRecoverableTax5': 0,
          'RetainageTaxBase1': 0,
          'RetainageTaxBase2': 0,
          'RetainageTaxBase3': 0,
          'RetainageTaxBase4': 0,
          'RetainageTaxBase5': 0,
          'RetainageTaxAmount1': 0,
          'RetainageTaxAmount2': 0,
          'RetainageTaxAmount3': 0,
          'RetainageTaxAmount4': 0,
          'RetainageTaxAmount5': 0,
          'FunctionalTaxBase1': 12,
          'FunctionalTaxBase2': 12,
          'FunctionalTaxBase3': 0,
          'FunctionalTaxBase4': 0,
          'FunctionalTaxBase5': 0,
          'FunctionalTaxAmount1': 0.75,
          'FunctionalTaxAmount2': 0.24,
          'FunctionalTaxAmount3': 0,
          'FunctionalTaxAmount4': 0,
          'FunctionalTaxAmount5': 0,
          'FunctionalRetainageTaxAmount1': 0,
          'FunctionalRetainageTaxAmount2': 0,
          'FunctionalRetainageTaxAmount3': 0,
          'FunctionalRetainageTaxAmount4': 0,
          'FunctionalRetainageTaxAmount5': 0,
          'FunctionalTaxRecoverableAmount1': 0,
          'FunctionalTaxRecoverableAmount2': 0,
          'FunctionalTaxRecoverableAmount3': 0,
          'FunctionalTaxRecoverableAmount4': 0,
          'FunctionalTaxRecoverableAmount5': 0,
          'FunctionalTaxExpenseSeparatelyAmount1': 0,
          'FunctionalTaxExpenseSeparatelyAmount2': 0,
          'FunctionalTaxExpenseSeparatelyAmount3': 0,
          'FunctionalTaxExpenseSeparatelyAmount4': 0,
          'FunctionalTaxExpenseSeparatelyAmount5': 0,
          'FunctionalTaxAllocatedTotal': 0.99,
          'FunctionalTaxAllocatedAmount1': 0.75,
          'FunctionalTaxAllocatedAmount2': 0.24,
          'FunctionalTaxAllocatedAmount3': 0,
          'FunctionalTaxAllocatedAmount4': 0,
          'FunctionalTaxAllocatedAmount5': 0,
          'TaxAllocatedAmount1': 0.75,
          'TaxAllocatedAmount2': 0.24,
          'TaxAllocatedAmount3': 0,
          'TaxAllocatedAmount4': 0,
          'TaxAllocatedAmount5': 0,
          'FunctionalCost': 0,
          'FunctionalDistributedAmount': 12,
          'FunctionalDistributionNetOfTaxes': 12,
          'FunctionalRetainageAmount': 0,
          'FunctionalRetainageTaxAllocated': 0,
          'RetainageTaxAllocated': 0,
          'FunctionalRetainageTaxExpensed': 0,
          'RetainageTaxExpensed': 0,
          'RetainageTaxTotal': 0,
          'TaxAmount1Total': 0.75,
          'TaxAmount2Total': 0.24,
          'TaxAmount3Total': 0,
          'TaxAmount4Total': 0,
          'TaxAmount5Total': 0,
          'CurrencyRetainageAmount': 0,
          'CurrencyRetainageDistributionAmount': 0,
          'FixedAsset': 'No',
          'SageFixedAssetsOrgid': '',
          'SageFixedAssetsDatabase': '',
          'SageFixedAssetsCompanyOrganization': '',
          'SageFixedAssetsTemplate': '',
          'SageFixedAssetsAssetDescript': 'Purchase of inventory',
          'SeparateAssets': 'No',
          'SageFixedAssetsQuantity': 0,
          'SageFixedAssetsUnitOfMeasur': '',
          'SageFixedAssetsAssetValue': 0,
          'SageFixedAssetsFunctionalAssetVa': 0,
          'EstimatedTaxWithheldAmount1': 0,
          'EstimatedTaxWithheldAmount2': 0,
          'EstimatedTaxWithheldAmount3': 0,
          'EstimatedTaxWithheldAmount4': 0,
          'EstimatedTaxWithheldAmount5': 0,
          'ReverseChargeAmount1': 0,
          'ReverseChargeAmount2': 0,
          'ReverseChargeAmount3': 0,
          'ReverseChargeAmount4': 0,
          'ReverseChargeAmount5': 0,
          'ReverseChargeable1': 'No',
          'ReverseChargeable2': 'No',
          'ReverseChargeable3': 'No',
          'ReverseChargeable4': 'No',
          'ReverseChargeable5': 'No',
          'InvoiceDetailOptionalFields': [
            {
              'BatchNumber': 28,
              'EntryNumber': 1,
              'LineNumber': 20,
              'OptionalField': 'DANGEROUS',
              'Value': '                                                           0',
              'InvoiceDetailOptionalFieldType': 'YesNo',
              'Length': 0,
              'Decimals': 0,
              'AllowBlank': false,
              'Validate': false,
              'ValueSet': 'Yes',
              'TypedValueFieldIndex': 25,
              'TextValue': '',
              'AmountValue': 0,
              'NumberValue': 0,
              'IntegerValue': 0,
              'YesNoValue': false,
              'DateValue': null,
              'TimeValue': '1899-12-30T00:00:00Z',
              'OptionalFieldDescription': 'Dangerous Item',
              'ValueDescription': '',
              'UpdateOperation': 'Unspecified'
            },
            {
              'BatchNumber': 28,
              'EntryNumber': 1,
              'LineNumber': 20,
              'OptionalField': 'EXTWARRANTY',
              'Value': '                                                           1',
              'InvoiceDetailOptionalFieldType': 'YesNo',
              'Length': 0,
              'Decimals': 0,
              'AllowBlank': false,
              'Validate': false,
              'ValueSet': 'Yes',
              'TypedValueFieldIndex': 25,
              'TextValue': '',
              'AmountValue': 0,
              'NumberValue': 0,
              'IntegerValue': 0,
              'YesNoValue': true,
              'DateValue': null,
              'TimeValue': '1899-12-30T00:00:00Z',
              'OptionalFieldDescription': 'Extended Warranty Available',
              'ValueDescription': '',
              'UpdateOperation': 'Unspecified'
            },
            {
              'BatchNumber': 28,
              'EntryNumber': 1,
              'LineNumber': 20,
              'OptionalField': 'QUANTITY',
              'Value': '0',
              'InvoiceDetailOptionalFieldType': 'Number',
              'Length': 0,
              'Decimals': 0,
              'AllowBlank': false,
              'Validate': false,
              'ValueSet': 'Yes',
              'TypedValueFieldIndex': 23,
              'TextValue': '',
              'AmountValue': 0,
              'NumberValue': 0,
              'IntegerValue': 0,
              'YesNoValue': false,
              'DateValue': null,
              'TimeValue': '1899-12-30T00:00:00Z',
              'OptionalFieldDescription': 'Quantity',
              'ValueDescription': '',
              'UpdateOperation': 'Unspecified'
            },
            {
              'BatchNumber': 28,
              'EntryNumber': 1,
              'LineNumber': 20,
              'OptionalField': 'UOM',
              'Value': '',
              'InvoiceDetailOptionalFieldType': 'Text',
              'Length': 10,
              'Decimals': 0,
              'AllowBlank': false,
              'Validate': false,
              'ValueSet': 'Yes',
              'TypedValueFieldIndex': 21,
              'TextValue': '',
              'AmountValue': 0,
              'NumberValue': 0,
              'IntegerValue': 0,
              'YesNoValue': false,
              'DateValue': null,
              'TimeValue': '1899-12-30T00:00:00Z',
              'OptionalFieldDescription': 'UOM',
              'ValueDescription': '',
              'UpdateOperation': 'Unspecified'
            },
            {
              'BatchNumber': 28,
              'EntryNumber': 1,
              'LineNumber': 20,
              'OptionalField': 'WARRANTYPRD',
              'Value': '90 Days',
              'InvoiceDetailOptionalFieldType': 'Text',
              'Length': 10,
              'Decimals': 0,
              'AllowBlank': true,
              'Validate': true,
              'ValueSet': 'Yes',
              'TypedValueFieldIndex': 21,
              'TextValue': '90 Days',
              'AmountValue': 0,
              'NumberValue': 0,
              'IntegerValue': 0,
              'YesNoValue': false,
              'DateValue': null,
              'TimeValue': '1899-12-30T00:00:00Z',
              'OptionalFieldDescription': 'Warranty Period',
              'ValueDescription': '90 Days',
              'UpdateOperation': 'Unspecified'
            }
          ],
          'UpdateOperation': 'Unspecified'
        }
      ],
      'InvoicePaymentSchedules': [
        {
          'BatchNumber': 28,
          'EntryNumber': 1,
          'PaymentNumber': 1,
          'DueDate': '2019-06-25T00:00:00Z',
          'AmountDue': 12.99,
          'DiscountDate': null,
          'DiscountAmount': 0,
          'FunctionalAmountDue': 12.99,
          'FunctionalDiscountAmount': 0,
          'UpdateOperation': 'Unspecified'
        }
      ],
      'InvoiceOptionalFields': [
        {
          'BatchNumber': 28,
          'EntryNumber': 1,
          'OptionalField': 'APPROVED',
          'Value': '',
          'InvoiceOptionalFieldType': 'Text',
          'Length': 3,
          'Decimals': 0,
          'AllowBlank': false,
          'Validate': false,
          'ValueSet': 'Yes',
          'TypedValueFieldIndex': 21,
          'TextValue': '',
          'AmountValue': 0,
          'NumberValue': 0,
          'IntegerValue': 0,
          'YesNoValue': false,
          'DateValue': null,
          'TimeValue': '1899-12-30T00:00:00Z',
          'OptionalFieldDescription': 'Approved',
          'ValueDescription': '',
          'UpdateOperation': 'Unspecified'
        },
        {
          'BatchNumber': 28,
          'EntryNumber': 1,
          'OptionalField': 'BACKORDER',
          'Value': '',
          'InvoiceOptionalFieldType': 'Text',
          'Length': 3,
          'Decimals': 0,
          'AllowBlank': false,
          'Validate': false,
          'ValueSet': 'Yes',
          'TypedValueFieldIndex': 21,
          'TextValue': '',
          'AmountValue': 0,
          'NumberValue': 0,
          'IntegerValue': 0,
          'YesNoValue': false,
          'DateValue': null,
          'TimeValue': '1899-12-30T00:00:00Z',
          'OptionalFieldDescription': 'Backorder',
          'ValueDescription': '',
          'UpdateOperation': 'Unspecified'
        },
        {
          'BatchNumber': 28,
          'EntryNumber': 1,
          'OptionalField': 'COURIER',
          'Value': '',
          'InvoiceOptionalFieldType': 'Text',
          'Length': 4,
          'Decimals': 0,
          'AllowBlank': false,
          'Validate': false,
          'ValueSet': 'Yes',
          'TypedValueFieldIndex': 21,
          'TextValue': '',
          'AmountValue': 0,
          'NumberValue': 0,
          'IntegerValue': 0,
          'YesNoValue': false,
          'DateValue': null,
          'TimeValue': '1899-12-30T00:00:00Z',
          'OptionalFieldDescription': 'Courier',
          'ValueDescription': '',
          'UpdateOperation': 'Unspecified'
        },
        {
          'BatchNumber': 28,
          'EntryNumber': 1,
          'OptionalField': 'DATERCVD',
          'Value': '00000000',
          'InvoiceOptionalFieldType': 'Date',
          'Length': 0,
          'Decimals': 0,
          'AllowBlank': false,
          'Validate': false,
          'ValueSet': 'Yes',
          'TypedValueFieldIndex': 26,
          'TextValue': '',
          'AmountValue': 0,
          'NumberValue': 0,
          'IntegerValue': 0,
          'YesNoValue': false,
          'DateValue': null,
          'TimeValue': '1899-12-30T00:00:00Z',
          'OptionalFieldDescription': 'Date Rcvd',
          'ValueDescription': '',
          'UpdateOperation': 'Unspecified'
        },
        {
          'BatchNumber': 28,
          'EntryNumber': 1,
          'OptionalField': 'UPSZONE',
          'Value': 'RED',
          'InvoiceOptionalFieldType': 'Text',
          'Length': 3,
          'Decimals': 0,
          'AllowBlank': true,
          'Validate': true,
          'ValueSet': 'Yes',
          'TypedValueFieldIndex': 21,
          'TextValue': 'RED',
          'AmountValue': 0,
          'NumberValue': 0,
          'IntegerValue': 0,
          'YesNoValue': false,
          'DateValue': null,
          'TimeValue': '1899-12-30T00:00:00Z',
          'OptionalFieldDescription': 'UPS Zone',
          'ValueDescription': 'Red Zone',
          'UpdateOperation': 'Unspecified'
        },
        {
          'BatchNumber': 28,
          'EntryNumber': 1,
          'OptionalField': 'USER',
          'Value': '',
          'InvoiceOptionalFieldType': 'Text',
          'Length': 60,
          'Decimals': 0,
          'AllowBlank': false,
          'Validate': false,
          'ValueSet': 'Yes',
          'TypedValueFieldIndex': 21,
          'TextValue': '',
          'AmountValue': 0,
          'NumberValue': 0,
          'IntegerValue': 0,
          'YesNoValue': false,
          'DateValue': null,
          'TimeValue': '1899-12-30T00:00:00Z',
          'OptionalFieldDescription': 'User',
          'ValueDescription': '',
          'UpdateOperation': 'Unspecified'
        }
      ],
      'UpdateOperation': 'Unspecified'
    }
  ],
  'UpdateOperation': 'Unspecified'
}";





            var bbb =
                @"{ 'status':'success','pages':[{'number':1,'height':792,'width':612,'clusterId':0,'keyValuePairs':[{'key':[{'text':'Number:','boundingBox':[407.4,747.9,440.3,747.9,440.3,736.5,407.4,736.5]
    }],'value':[{'text':'IN0000000000002','boundingBox':[499.1,747.4,564.9,747.4,564.9,736.5,499.1,736.5],'confidence':1.0}]},{'key':[{'text':'Amount','boundingBox':[530.8,557.6,564.7,557.6,564.7,544.7,530.8,544.7]}],'value':[{'text':'599.90','boundingBox':[540.3,539.0,564.8,539.0,564.8,528.1,540.3,528.1],'confidence':1.0},{'text':'44.73','boundingBox':[544.8,518.7,564.8,518.7,564.8,507.8,544.8,507.8],'confidence':1.0},{'text':'151.05','boundingBox':[540.3,498.3,564.8,498.3,564.8,487.4,540.3,487.4],'confidence':1.0},{'text':'748.50','boundingBox':[540.3,478.0,564.8,478.0,564.8,467.1,540.3,467.1],'confidence':1.0},{'text':'265.50','boundingBox':[540.3,457.6,564.8,457.6,564.8,446.7,540.3,446.7],'confidence':1.0}]}],'tables':[{'id':'table_0','columns':[{'header':[{'text':'Reference - P.O. No.','boundingBox':[62.2,590.6,148.8,590.6,148.8,577.7,62.2,577.7]}],'entries':[[{'text':'__emptycell__','boundingBox':[59.0,576.0,188.0,576.0,188.0,565.0,59.0,565.0],'confidence':1.0}]]},{'header':[{'text':'Customer No.','boundingBox':[192.0,590.6,251.0,590.6,251.0,577.7,192.0,577.7]}],'entries':[[{'text':'1200','boundingBox':[192.0,578.0,209.8,578.0,209.8,567.1,192.0,567.1],'confidence':1.0}]]},{'header':[{'text':'Salesperson','boundingBox':[272.2,590.6,325.8,590.6,325.8,577.7,272.2,577.7]}],'entries':[[{'text':'BB','boundingBox':[272.2,578.0,282.9,578.0,282.9,567.1,272.2,567.1],'confidence':1.0}]]},{'header':[{'text':'Ship Via','boundingBox':[354.8,590.6,390.1,590.6,390.1,577.7,354.8,577.7]}],'entries':[[{'text':'Cross-Country Trucking Lines','boundingBox':[354.8,578.0,460.1,578.0,460.1,567.1,354.8,567.1],'confidence':1.0}]]},{'header':[{'text':'Terms Code','boundingBox':[465.8,590.6,517.1,590.6,517.1,577.7,465.8,577.7]}],'entries':[[{'text':'DUETBL','boundingBox':[465.8,578.0,497.3,578.0,497.3,567.1,465.8,567.1],'confidence':1.0}]]}]}]}],'errors':[]}";

            var aaa = JObject.Parse(bbb);

            var haha = aaa.SelectToken("$.pages[?(@.keyValuePairs)]").SelectToken("$.keyValuePairs").Select(m => m)
                .ToList();

            var qqq = JObject.Parse(ppp);
            qqq["Invoices"][0]["DocumentNumber"] =
                aaa.SelectToken("$.pages[?(@.keyValuePairs)]").SelectToken("$.keyValuePairs").Select(m => m).ToList()[0]
                    .SelectTokens("$.value[?(@.text)]").ToList()[0]["text"] + "8";


           var sdkdk = aaa.SelectToken("$.pages[?(@.keyValuePairs)]").SelectToken("$.keyValuePairs").Select(m => m).ToList()[1]
                    .SelectTokens("$.value[?(@.text)]").ToList();

           var amounts = sdkdk.Select(m => m["text"]).ToList();

           foreach (var detail in amounts)
           {
               //create details
           }



            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(qqq.ToString());



            var response = _client.Post(request);


            return true;
        }
    }
}