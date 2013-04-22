using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectQueryTests
{
    struct TestStructure
    {
        public string Name;
        public int Age;
        public bool IsMale;
        public DateTime BirthDate;

        public CustomType CType;

    }


    struct TestNull
    {
        public int? data;


        public TestNull(int x)
        {
            data = x;
        }
    }



    struct TestString
    {
        public string Name;


        public TestString(string name)
        {
            this.Name = name;
        }

    }

    public class CutomPlaceHolder
    {

        private CustomType ct;


        public CutomPlaceHolder()
        {
            ct = new CustomType();
        }

        public int Things
        {
            get
            {
                return 6;
            }
        }



        public CustomType MyTypes
        {
            get
            {
                return ct;
            }
            set
            {
                ct = value;
            }
        }
    }


    public struct CustomType : IComparable
    {
        public int X;



        public CustomType(int x)
        {
            this.X = x;
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            CustomType ct = (CustomType)obj;

            if (this.X > ct.X)
            {
                return 1;
            }

            if (ct.X == this.X)
            {
                return 0;
            }

            return -1;
        }
        #endregion
    }
}
