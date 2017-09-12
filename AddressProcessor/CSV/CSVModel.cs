namespace AddressProcessing.CSV
{
    using System.Collections.Generic;

    public class CSVModel
    {
        private List<string> _columns;

        public CSVModel(List<string> columns)
        {
            this._columns = columns;
        }

        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public CSVModel Build()
        {
            AddName();
            AddAddress();
            AddPhone();
            AddEmail();

            return this;
        }

        private void AddName()
        {
            if (_columns.Count >= 1)
            {
                Name = _columns[0];
            }
        }

        private void AddAddress()
        {
            if (_columns.Count >= 2)
            {
                Address = _columns[1];
            }
        }

        private void AddPhone()
        {
            if (_columns.Count >= 3)
            {
                Phone = _columns[2];
            }
        }

        private void AddEmail()
        {
            if (_columns.Count >= 4)
            {
                Email = _columns[3];
            }

        }
    }    
}