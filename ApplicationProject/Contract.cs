using System;
using System.Collections.Generic;

namespace ApplicationProject
{
    public class Contract
    {
        public string Artist { get; set; }
        public string Title { get; set; }
        public List<string> Usages { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
                return true;

            var secondContract = obj as Contract;
            if (obj == null)
                return false;

            return String.Equals(this.Artist, secondContract.Artist, StringComparison.InvariantCultureIgnoreCase)
                && String.Equals(this.Title, secondContract.Title, StringComparison.InvariantCultureIgnoreCase);
        }

        public override int GetHashCode()
        {
            return @"{Artist}|||{Title}".GetHashCode();
        }
    }
}
