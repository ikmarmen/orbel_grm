using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace ApplicationProject
{
    public class MusicContractService
    {
        public List<Contract> Contracts { get; private set; }

        public List<Partner> Partners { get; private set; }

        public MusicContractService()
        {
            Contracts = ReadContracts();
            Partners = ReadPartners();
        }

        private static List<Contract> ReadContracts()
        {
            if (!File.Exists(Constants.MusicContractsFileName))
                throw new Exception("Missing Music Contracts File: " + Constants.PartnerContractsFileName);

            return File.ReadAllLines(Constants.MusicContractsFileName).Skip(1).Select(i =>
                {
                    var data = i.Split('|');
                    var date = DateTime.MinValue;
                    var contract = new Contract
                    {
                        Artist = data[0].Trim(),
                        Title = data[1].Trim(),
                        Usages = SplitTrim(data[2], ","),
                        StartDate = DateTime.Parse(data[3].Trim())
                    };

                    if (DateTime.TryParse(data[4], out date))
                        contract.EndDate = date;

                    return contract;
                }).ToList();

        }
        private static List<Partner> ReadPartners()
        {
            if (!File.Exists(Constants.PartnerContractsFileName))
                throw new Exception("Missing Partner Contracts File: " + Constants.PartnerContractsFileName);

            return File.ReadAllLines(Constants.PartnerContractsFileName).Skip(1).Select(i =>
            {
                var data = i.Split('|');
                return new Partner
                {
                    Name = data[0].Trim(),
                    Usages = SplitTrim(data[1], ",")
                };
            }).ToList();
        }

        public List<Contract> Search(string partner, DateTime date)
        {
            // Do NOT return empty list on error since empty list is a valid result 
            //   meaning search succeeded and returned no result!
            if (Partners == null)
                throw new Exception("Partners List Not Found!");
            if (Contracts == null)
                throw new Exception("Contracts List Not Found!");

            var partnerUsages = Partners
                .Where(p => String.Equals(p.Name, partner, StringComparison.InvariantCultureIgnoreCase))
                .SelectMany(p => p.Usages)
                .Distinct()
                .ToList();

            return Contracts
                .Where(c => 
                    c.Usages.Any(u => partnerUsages.Any(pu => String.Equals(pu, u, StringComparison.InvariantCultureIgnoreCase))) 
                        && c.StartDate <= date 
                        && (!c.EndDate.HasValue || c.EndDate.Value >= date)).ToList();
        }

        /// <summary>
        /// Splits and Trims the text
        /// </summary>
        /// <param name="text">Text to split</param>
        /// <param name="delimeters">Delimeters to split by</param>
        /// <returns>List of trimmed components of text split by delimeters</returns>
        private static List<string> SplitTrim(string text, string delimeters)
        {
            return text.Split(delimeters.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(u => u.Trim()).ToList();
        }
    }

}
