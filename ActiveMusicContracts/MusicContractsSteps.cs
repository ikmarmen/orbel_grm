using System;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using ApplicationProject;
using System.IO;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace ActiveMusicContracts
{
    [Binding]
    public class MusicContractsSteps
    {
        private MusicContractService _musicContractService;
        private List<Contract> _searchResults;
        
        [Given(@"the supplied reference data")]
        public void GivenTheSuppliedReferenceData()
        {
            try
            {
                _musicContractService = new MusicContractService();
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }

            //if (_musicContractService?.Contracts?.Count == 0 || _musicContractService?.Partners?.Count == 0)
            var dataIsLoaded = _musicContractService.Contracts != null && _musicContractService.Partners != null;
            if (!dataIsLoaded)
                Assert.Fail("Data file(s) not found!");

            var dataIsSupplied = dataIsLoaded && _musicContractService.Contracts.Count > 0 && _musicContractService.Partners.Count > 0;
            if (!dataIsSupplied)
                Assert.Fail("At leat one data file is empty!");
        }

        [When(@"user perform search by (.*) (.*)")]
        public void WhenUserPerformSearchByITunes(string partnerName, string effectiveDateString)
        {
            DateTime effectiveDate;
            if (!DateTime.TryParse(effectiveDateString, out effectiveDate))
                Assert.Fail("Invalid Effective Date");

            try
            {
                _searchResults = _musicContractService.Search(partnerName, effectiveDate).OrderBy(i => i.Artist).ThenBy(i => i.Title).ToList();
            }
            catch(Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        [Then(@"the output should be")]
        public void ThenTheOutputShouldBe(Table table)
        {
            if (_searchResults == null)
                Assert.Fail("Null search results!");

            var expectedContracts = table.CreateSet<Contract>().OrderBy(i => i.Artist).ThenBy(i => i.Title).ToList();
            Assert.AreEqual(expectedContracts.Count, _searchResults.Count);
            for (int i = 0; i < expectedContracts.Count; i++)
            {
                Assert.IsTrue(expectedContracts[i].Equals(_searchResults[i]));
            }
        }
    }

    [Binding]
    public class DeployFiles
    {
        [BeforeScenario("copyDataFiles")]
        public void CopyFiles()
        {
            Console.WriteLine("Copy files");
            var assemblyPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            File.Copy(Path.Combine(assemblyPath, "Data", Constants.MusicContractsFileName), Path.Combine(Directory.GetCurrentDirectory(), Constants.MusicContractsFileName), true);
            File.Copy(Path.Combine(assemblyPath, "Data", Constants.PartnerContractsFileName), Path.Combine(Directory.GetCurrentDirectory(), Constants.PartnerContractsFileName), true);
        }
    }
}
