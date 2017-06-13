namespace Ops.Regions
{
    using System.Collections.Generic;
    using System.Xml;

    public static class Region
    {
        #region 获取基本数据

        private static IEnumerable<Province> provinces;
        public static IEnumerable<Province> Provinces
        {
            get
            {
                if (provinces == null)
                {
                    provinces = GetProvinces();
                }
                return provinces;
            }
        }

        private static IEnumerable<City> cities;
        public static IEnumerable<City> Cities
        {
            get
            {
                if (cities == null)
                {
                    cities = GetCitys();
                }
                return cities;
            }
        }

        private static IEnumerable<District> districts;
        public static IEnumerable<District> Districts
        {
            get
            {
                if (districts == null)
                {
                    districts = GetDistricts();
                }
                return districts;
            }
        }


        private static IEnumerable<Province> GetProvinces()
        {
            XmlDocument xd = new XmlDocument();
            xd.LoadXml(Define.PROVINCES);

            string name;

            foreach (XmlNode xn in xd.SelectNodes("/items/item"))
            {
                name = xn.Attributes[1].Value;
                yield return new Province
                {
                    ID = int.Parse(xn.Attributes[0].Value),
                    Name = name,
                    Text = xn.InnerText.Trim() == "" ? name : xn.InnerText.Trim()
                };
            }
        }


        private static IEnumerable<City> GetCitys()
        {
            XmlDocument xd = new XmlDocument();
            xd.LoadXml(Define.CITIES);

            string name;

            foreach (XmlNode xn in xd.SelectNodes("/items/item"))
            {
                name = xn.Attributes[1].Value;
                yield return new City
                {
                    ID = int.Parse(xn.Attributes[0].Value),
                    Pid = int.Parse(xn.Attributes[2].Value),
                    Name = name,
                    Zip = xn.Attributes[3].Value,
                    Text = xn.InnerText.Trim() == "" ? name : xn.InnerText.Trim()
                };
            }
        }

        private static IEnumerable<District> GetDistricts()
        {
            XmlDocument xd = new XmlDocument();
            xd.LoadXml(Define.DISTRICTS);

            string name;

            foreach (XmlNode xn in xd.SelectNodes("/items/item"))
            {
                name = xn.Attributes[1].Value;
                yield return new District
                {
                    ID = int.Parse(xn.Attributes[0].Value),
                    Cid = int.Parse(xn.Attributes[2].Value),
                    Name = name,
                    Text = xn.InnerText.Trim() == "" ? name : xn.InnerText.Trim()
                };
            }
        }

        #endregion


        public static IEnumerable<City> GetCities(int provinceID)
        {
            foreach (City c in Cities)
            {
                if (c.Pid == provinceID)
                    yield return c;
            }
        }

        public static IEnumerable<District> GetDistricts(int cityID)
        {
            foreach (District d in Districts)
            {
                if (d.Cid == cityID)
                    yield return d;
            }
        }

        public static Province GetProvince(string provinceName)
        {
            foreach (Province p in Provinces)
            {
                if (string.Compare(p.Name, provinceName,true)==0)
                {
                    return p;
                }
            }
            return default(Province);
        }

        public static City GetCity(string cityName)
        {
            foreach (City p in Cities)
            {
                if (string.Compare(p.Name, cityName, true) == 0)
                {
                    return p;
                }
            }
            return default(City);
        }
    }
}