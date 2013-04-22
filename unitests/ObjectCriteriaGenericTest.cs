
using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
//using voidsoft.DataBlock;
using voidsoft.DataBlock.ObjectQuery;
using System.Diagnostics;




namespace ObjectQueryTests
{

    [TestFixture()]
    public class ObjectCriteriaGenericTest
    {
        /// <summary>
        /// Gets the test data.
        /// </summary>
        /// <returns></returns>
        private List<TestStructure> GetTestData()
        {
            List<TestStructure> list = new List<TestStructure>();

            TestStructure t = new TestStructure();
            t.Age = 56;
            t.BirthDate = new DateTime(1990, 2, 12);
            t.IsMale = false;
            t.Name = "Ionic";
            t.CType = new CustomType (2);

            list.Add(t);

            TestStructure tt = new TestStructure();
            tt.Age = 15;
            tt.BirthDate = new DateTime(1290, 3, 11);
            tt.IsMale = true;
            tt.Name = "Jaleb";
            list.Add(tt);


            TestStructure tv = new TestStructure();
            tv.Age = 12;
            tv.BirthDate = new DateTime(1950, 7, 12);
            tv.IsMale = true;
            tv.Name = "Mihai";

            list.Add(tv);

            TestStructure tg = new TestStructure();
            tg.Age = 156;
            tg.BirthDate = new DateTime(1936, 2, 12);
            tg.IsMale = false;
            tg.Name = "Snicker";
            tg.CType = new  CustomType(342);

            list.Add(tg);

            TestStructure tl = new TestStructure();
            tl.Age = 34;
            tl.BirthDate = new DateTime(1922, 2, 12);
            tl.IsMale = false;
            tl.Name = "Goguta";
            list.Add(tl);


            TestStructure tlk = new TestStructure();
            tlk.Age = 89;
            tlk.BirthDate = new DateTime(1978, 2, 12);
            tlk.IsMale = true;
            tlk.Name = "Goguta or Minciougutza";
            tlk.CType = new CustomType(389);
            list.Add(tlk);


            return list;
        }



        #region nullable types tests
        [Test]
        public void TestIsNullData()
        {
            TestNull[] t = { new TestNull(3), new TestNull(), new TestNull(1) };

            ObjectQuery<TestNull> qlink = new ObjectQuery<TestNull>();
            TestNull[] tk = (TestNull[]) qlink.Select(t, "data = null");

            Console.WriteLine("TestNull");
        }


        [Test]
        public void TestIsNotNullData()
        {
            TestNull[] t = { new TestNull(3), new TestNull(), new TestNull(1) };

            ObjectQuery<TestNull> qlink = new ObjectQuery<TestNull>();
            TestNull[] tk = (TestNull[]) qlink.Select(t, "data <> null");

            Console.WriteLine("TestIsNotNull");
        }
        
        #endregion


        #region Numeric

        #region Select
        [Test]
        public void TestSelectNumericDifferent()
        {
            Process[] proc = Process.GetProcesses();

            ObjectQuery<Process> q = new ObjectQuery<Process>();
            Process[] pc = (Process[]) q.Select(proc, "BasePriority <> 3");

            Console.WriteLine("TestSelectNumericDifferent");
            foreach (Process var in pc)
            {
                Console.WriteLine(var.ProcessName);
            }
        }

        [Test]
        public void TestSelectNumericBigger()
        {
            Process[] proc = Process.GetProcesses();

            ObjectQuery<Process> q = new ObjectQuery<Process>();
            Process[] pc = (Process[]) q.Select(proc, "BasePriority > 3");

            Console.WriteLine("TestSelectNumericBigger");
            foreach (Process var in pc)
            {
                Console.WriteLine(var.ProcessName);
            }
        } 

        [Test]
        public void TestSelectNumericBetween()
        {
            Process[] proc = Process.GetProcesses();

            ObjectQuery<Process> q = new ObjectQuery<Process>();
            Process[] pc = (Process[]) q.Select(proc, "BasePriority between 5 200");

            Console.WriteLine("TestSelectIntegerBetween");
            foreach (Process var in pc)
            {
                Console.WriteLine(var.ProcessName);
            }
        }


        [Test]
        public void TestSelectNumericEqual()
        {
            List<TestStructure> lst = this.GetTestData();

            ObjectQuery<TestStructure> q = new ObjectQuery<TestStructure>();
            TestStructure[] tk = (TestStructure[]) q.Select(lst, "Age = 15");

            Console.WriteLine("TestSelectNumericEqual");
        }

        [Test]
        public void TestSelectNumericBiggerOrEqual()
        {
            Process[] proc = Process.GetProcesses();

            ObjectQuery<Process> q = new ObjectQuery<Process>();
            Process[] pc = (Process[]) q.Select(proc, "BasePriority >= 3");

            Console.WriteLine("TestSelectNumericBiggerOrEqual");
            foreach (Process var in pc)
            {
                Console.WriteLine(var.ProcessName);
            }
        }


        [Test]
        public void TestSelectNumericSmaller()
        {
            Process[] proc = Process.GetProcesses();

            ObjectQuery<Process> q = new ObjectQuery<Process>();
            Process[] pc = (Process[]) q.Select(proc, "BasePriority < 3");

            Console.WriteLine("TestSelectNumericSmaller");
            foreach (Process var in pc)
            {
                Console.WriteLine(var.ProcessName);
            }
        }


        [Test]
        public void TestSelectNumericSmallerOrEqual()
        {
            Process[] proc = Process.GetProcesses();

            ObjectQuery<Process> q = new ObjectQuery<Process>();
            Process[] pc = (Process[]) q.Select(proc, "BasePriority <= 3");

            Console.WriteLine("TestSelectNumericSmallerOrEqual");
            foreach (Process var in pc)
            {
                Console.WriteLine(var.ProcessName);
            }
        }

        #endregion

        #region Sum
        [Test]
        public void SumNumeric()
        {
            List<TestStructure> t = this.GetTestData();

            ObjectQuery<TestStructure> oq = new ObjectQuery<TestStructure>();
            decimal dc = oq.Sum(t, "Age");

            Console.WriteLine(dc);
            Assert.IsTrue(dc > 0);
        }



        [Test]
        public void SumWithConditionNumeric()
        {
            List<TestStructure> t = this.GetTestData();

            ObjectQuery<TestStructure> oq = new ObjectQuery<TestStructure>();
            decimal dc = oq.Sum(t, "Age", "Age > 50 and Name like I% and IsMale = false");

            Console.WriteLine(dc);
            Assert.IsTrue(dc == 56);
        }

        #endregion

        #region Min

        [Test]
        public void TestMinNumeric()
        {
            List<TestStructure> t = this.GetTestData();

            ObjectQuery<TestStructure> oq = new ObjectQuery<TestStructure>();
            decimal dc = (decimal)oq.Min(t, "Age");

            Console.WriteLine(dc);
            Assert.IsTrue(dc > 0);
        }

        [Test]
        public void TestMinNumericConditional()
        {
            List<TestStructure> t = this.GetTestData();

            ObjectQuery<TestStructure> oq = new ObjectQuery<TestStructure>();
            decimal dc = (decimal)oq.Min(t, "Age", "Age > 5");

            Console.WriteLine(dc);
            Assert.IsTrue(dc > 0);
        }
        #endregion

        #region Max
        [Test]
        public void TestMaxNumeric()
        {
            List<TestStructure> t = this.GetTestData();

            ObjectQuery<TestStructure> oq = new ObjectQuery<TestStructure>();
            decimal dc = (decimal)oq.Max(t, "Age");

            Console.WriteLine(dc);
            Assert.IsTrue(dc > 0);
        }


        [Test]
        public void TestMaxNumericConditional()
        {
            List<TestStructure> t = this.GetTestData();

            ObjectQuery<TestStructure> oq = new ObjectQuery<TestStructure>();
            decimal dc = (decimal)oq.Max(t, "Age", "Age > 5");

            Console.WriteLine(dc);
            Assert.IsTrue(dc > 0);
        }
        #endregion

        #endregion


        #region String

        [Test]
        public void TestStringWithSpaces()
        {
            TestString[] ts = new TestString[2];
            ts[0] = new TestString("coconut brown");
            ts[1] = new TestString("blue violet");


            ObjectQuery<TestString> oq = new ObjectQuery<TestString>();
            TestString[] s = (TestString[]) oq.Select(ts, "Name = [blue violet]");

            Console.WriteLine("TestStringWithSpaces");

            foreach (TestString var in s)
            {
                Console.WriteLine(var.Name);
            }
        }


        [Test]
        public void TestStringWithMultipleSpaces()
        {
            TestString[] ts = new TestString[2];
            ts[0] = new TestString(" coconut brown");
            ts[1] = new TestString(" blue violet slick ");


            ObjectQuery<TestString> oq = new ObjectQuery<TestString>();
            TestString[] s = (TestString[]) oq.Select(ts, "Name = [ blue violet slick ]");

            Console.WriteLine("TestStringWithMultipleSpaces");

            foreach (TestString var in s)
            {
                Console.WriteLine(var.Name);
            }
        }


        [Test]
        public void TestSelectStringEqual()
        {
            Process[] proc = Process.GetProcesses();

            ObjectQuery<Process> q = new ObjectQuery<Process>();
            Process[] pc = (Process[]) q.Select(proc, "MachineName = MariusG");

            Console.WriteLine("SelectStringEqual");

            foreach (Process var in pc)
            {
                Console.WriteLine(var.ProcessName);
            }
        }


        [Test]
        public void TestSelectStringDifferent()
        {
            Process[] proc = Process.GetProcesses();

            ObjectQuery<Process> q = new ObjectQuery<Process>();
            Process[] pc = (Process[]) q.Select(proc, "MachineName <> MariusG");

            Console.WriteLine("SelectStringDifferent");

            foreach (Process var in pc)
            {
                Console.WriteLine(var.ProcessName);
            }
        }


        [Test]
        public void TestStringSelectWithMultipleSpaces()
        {
            List<TestString> lk = new List<TestString>();
            lk.Add(new TestString(" Abraham and Ionela Lincoln"));
            lk.Add(new TestString("Postasu"));
            lk.Add(new TestString("Scufitza rosie and alba ca zapada"));

            ObjectQuery<TestString> oq = new ObjectQuery<TestString>();
            TestString[] tst =  (TestString[])  oq.Select(lk, "Name like [Scufitza rosie and alba%]");

            Console.WriteLine("TestStringSelectWithMultipleSpaces");

            foreach (TestString var in tst)
            {
                Console.WriteLine(var.Name);
            }

            Assert.IsTrue(tst.Length == 1);
        }



        [Test]
        public void TestSelectStringLikeStart()
        {
            Process[] proc = Process.GetProcesses();

            ObjectQuery<Process> q = new ObjectQuery<Process>();
            Process[] pc = (Process[]) q.Select(proc, "ProcessName like C%");

            Console.WriteLine("SelectStringLikeStart");

            foreach (Process var in pc)
            {
                Console.WriteLine(var.ProcessName);
            }
        }


        [Test]
        public void TestSelectStringLikeEnd()
        {
            Process[] proc = Process.GetProcesses();

            ObjectQuery<Process> q = new ObjectQuery<Process>();
            Process[] pc = (Process[]) q.Select(proc, "ProcessName like %e");

            Console.WriteLine("SelectStringLikeEnd");

            foreach (Process var in pc)
            {
                Console.WriteLine(var.ProcessName);
            }
        }

        [Test]
        public void TestSelectStringLikeMiddle()
        {
            Process[] proc = Process.GetProcesses();

            ObjectQuery<Process> q = new ObjectQuery<Process>();
            Process[] pc = (Process[]) q.Select(proc, "ProcessName like S%e");

            Console.WriteLine("SelectStringLikeMiddle");

            foreach (Process var in pc)
            {
                Console.WriteLine(var.ProcessName);
            }
        }

        #endregion


        #region DateTime

        #region Select
        [Test]
        public void TestSelectDateTimeEquality()
        {
            List<TestStructure> list = this.GetTestData();

            ObjectQuery<TestStructure> oq = new ObjectQuery<TestStructure>();
            TestStructure[] tst = (TestStructure[])  oq.Select(list, "BirthDate = [1990, 2, 12]");

            Console.WriteLine("TestSelectDateTimeEquality");

            foreach (TestStructure var in tst)
            {
                Console.WriteLine(var.BirthDate.ToShortDateString());
            }
        }


        [Test]
        public void TestSelectDateTimeDifferent()
        {
            List<TestStructure> list = this.GetTestData();

            ObjectQuery<TestStructure> oq = new ObjectQuery<TestStructure>();
            TestStructure[] tst = (TestStructure[]) oq.Select(list, "BirthDate <> [1990, 2, 12]");

            Console.WriteLine("TestSelectDateTimeDifferent");

            foreach (TestStructure var in tst)
            {
                Console.WriteLine(var.BirthDate.ToShortDateString());
            }
        }


        [Test]
        public void TestSelectDateTimeBigger()
        {
            List<TestStructure> list = this.GetTestData();

            ObjectQuery<TestStructure> oq = new ObjectQuery<TestStructure>();
            TestStructure[] tst = (TestStructure[]) oq.Select(list, "BirthDate > [1920, 2, 12]");

            Console.WriteLine("TestSelectDateTimeBigger");

            foreach (TestStructure var in tst)
            {
                Console.WriteLine(var.BirthDate.ToShortDateString());
            }
        }


        [Test]
        public void TestSelectDateTimeBiggerOrEqual()
        {
            List<TestStructure> list = this.GetTestData();

            ObjectQuery<TestStructure> oq = new ObjectQuery<TestStructure>();
            TestStructure[] tst = (TestStructure[]) oq.Select(list, "BirthDate >= [1920, 2, 12]");

            Console.WriteLine("TestSelectDateTimeBiggerOrEqual");

            foreach (TestStructure var in tst)
            {
                Console.WriteLine(var.BirthDate.ToShortDateString());
            }
        }


        [Test]
        public void TestSelectDateTimeSmallOrEqual()
        {
            List<TestStructure> list = this.GetTestData();

            ObjectQuery<TestStructure> oq = new ObjectQuery<TestStructure>();
            TestStructure[] tst = (TestStructure[]) oq.Select(list, "BirthDate <= [1990, 2, 12]");

            Console.WriteLine("TestSelectDateTimeSmallOrEqual");

            foreach (TestStructure var in tst)
            {
                Console.WriteLine(var.BirthDate.ToShortDateString());
            }
        }

        [Test]
        public void TestSelectDateTimeSmall()
        {
            List<TestStructure> list = this.GetTestData();

            ObjectQuery<TestStructure> oq = new ObjectQuery<TestStructure>();
            TestStructure[] tst = (TestStructure[]) oq.Select(list, "BirthDate < [1990, 2, 12]");

            Console.WriteLine("TestSelectDateTimeSmall");

            foreach (TestStructure var in tst)
            {
                Console.WriteLine(var.BirthDate.ToShortDateString());
            }
        }


        [Test]
        public void TestSelectDateTimeBetween()
        {
            List<TestStructure> list = this.GetTestData();

            ObjectQuery<TestStructure> oq = new ObjectQuery<TestStructure>();
            TestStructure[] tst = (TestStructure[]) oq.Select(list, "BirthDate between [1900,2,12] [2000,1,1]");

            Console.WriteLine("TestSelectDateTimeBetween");

            foreach (TestStructure var in tst)
            {
                Console.WriteLine(var.BirthDate.ToShortDateString());
            }
        }
        #endregion

        #region MinMax
        [Test]
        public void TestMinDateTime()
        {
            List<TestStructure> list = this.GetTestData();

            ObjectQuery<TestStructure> oq = new ObjectQuery<TestStructure>();
            DateTime dt = (DateTime) oq.Min(list, "BirthDate");

            Console.WriteLine("TestMinDateTime");

            Assert.IsTrue(dt == new DateTime(1290, 3, 11));
        }



        [Test]
        public void TestMinDateTimeCondition()
        {
            List<TestStructure> list = this.GetTestData();

            ObjectQuery<TestStructure> oq = new ObjectQuery<TestStructure>();
            DateTime dt = (DateTime)oq.Min(list, "BirthDate", "Age > 0");

            Console.WriteLine("TestMinDateTime");

            Assert.IsTrue(dt == new DateTime(1290, 3, 11));
        }


        [Test]
        public void TestMaxDateTime()
        {
            List<TestStructure> t = this.GetTestData();

            ObjectQuery<TestStructure> oq = new ObjectQuery<TestStructure>();
            DateTime dc = (DateTime)oq.Max(t, "BirthDate");

            Console.WriteLine("TestMaxDateTime");

            Assert.IsTrue(dc == new DateTime(1990, 2,12));
        }

        [Test]
        public void TestMaxDateTimeConditional()
        {
            List<TestStructure> t = this.GetTestData();

            ObjectQuery<TestStructure> oq = new ObjectQuery<TestStructure>();
            DateTime dc = (DateTime)oq.Max(t, "BirthDate", "Age > 1");

            Console.WriteLine("TestMaxDateTime");

            Assert.IsTrue(dc == new DateTime(1990, 2, 12));
        }


        #endregion
        
        #endregion


        #region Bool
        [Test]
        public void TestSelectDiferentBool()
        {
            Process[] proc = Process.GetProcesses();

            ObjectQuery<Process> q = new ObjectQuery<Process>();
            Process[] pc = (Process[]) q.Select(proc, "Responding <> true");

            Console.WriteLine("SelectDiferentBool");
            foreach (Process var in pc)
            {
                Console.WriteLine(var.ProcessName);
            }
        }


        [Test]
        public void TestSelectEqualBool()
        {
            Process[] proc = Process.GetProcesses();

            ObjectQuery<Process> q = new ObjectQuery<Process>();
            Process[] pc = (Process[]) q.Select(proc, "Responding = true");

            Console.WriteLine("SelectEqualBool");
            foreach (Process var in pc)
            {
                Console.WriteLine(var.ProcessName);
            }

            Assert.IsTrue(pc.Length > 0);
        }

        #endregion


        #region IComparable tests

        #region Min tests
        [Test]
        public void TestMinIcomparable()
        {

            CutomPlaceHolder c = new CutomPlaceHolder();
            c.MyTypes = new CustomType(8);


            CutomPlaceHolder cc = new CutomPlaceHolder();
            cc.MyTypes = new CustomType(17);

            CutomPlaceHolder ccc = new CutomPlaceHolder();
            ccc.MyTypes = new CustomType(23);


            List<CutomPlaceHolder> list = new List<CutomPlaceHolder>();
            list.Add(c);
            list.Add(cc);
            list.Add(ccc);


            ObjectQuery<CutomPlaceHolder> oq = new ObjectQuery<CutomPlaceHolder>();
            object k = oq.Min(list, "MyTypes");

            CutomPlaceHolder kl = (CutomPlaceHolder)k;

            Assert.IsTrue(kl.MyTypes.X == 8);
        }


        #endregion

        #region Max tests
        [Test]
        public void TestMaxIcomparable()
        {
            CutomPlaceHolder c = new CutomPlaceHolder();
            c.MyTypes = new CustomType(8);


            CutomPlaceHolder cc = new CutomPlaceHolder();
            cc.MyTypes = new CustomType(17);

            CutomPlaceHolder ccc = new CutomPlaceHolder();
            ccc.MyTypes = new CustomType(23);


            List<CutomPlaceHolder> list = new List<CutomPlaceHolder>();
            list.Add(c);
            list.Add(cc);
            list.Add(ccc);


            ObjectQuery<CutomPlaceHolder> oq = new ObjectQuery<CutomPlaceHolder>();
            object k = oq.Max(list, "MyTypes");

            CutomPlaceHolder kl = (CutomPlaceHolder)k;

            Assert.IsTrue(kl.MyTypes.X == 23);
        }
        #endregion



        #region selects

        [Test]
        public void TestSelectIComparableEquality()
        {
            CutomPlaceHolder c = new CutomPlaceHolder();
            c.MyTypes = new CustomType(8);


            CutomPlaceHolder cc = new CutomPlaceHolder();
            cc.MyTypes = new CustomType(17);

            CutomPlaceHolder ccc = new CutomPlaceHolder();
            ccc.MyTypes = new CustomType(23);


            List<CutomPlaceHolder> list = new List<CutomPlaceHolder>();
            list.Add(c);
            list.Add(cc);
            list.Add(ccc);


            ObjectQuery<CutomPlaceHolder> oq = new ObjectQuery<CutomPlaceHolder>();
            object k = oq.Select(list, "MyTypes = 8");

            CutomPlaceHolder[] kl = (CutomPlaceHolder[])k;

            Assert.IsTrue(kl[0].MyTypes.X == 8);
        }



        [Test]
        public void TestSelectIComparableBiggerOrEqual()
        {
            CutomPlaceHolder c = new CutomPlaceHolder();
            c.MyTypes = new CustomType(8);


            CutomPlaceHolder cc = new CutomPlaceHolder();
            cc.MyTypes = new CustomType(17);

            CutomPlaceHolder ccc = new CutomPlaceHolder();
            ccc.MyTypes = new CustomType(23);


            List<CutomPlaceHolder> list = new List<CutomPlaceHolder>();
            list.Add(c);
            list.Add(cc);
            list.Add(ccc);


            ObjectQuery<CutomPlaceHolder> oq = new ObjectQuery<CutomPlaceHolder>();
            object k = oq.Select(list, "MyTypes >= 17");

            CutomPlaceHolder[] kl = (CutomPlaceHolder[])k;

            Assert.IsTrue(kl.Length == 2);
        }


        [Test]
        public void TestSelectIComparableBigger()
        {
            CutomPlaceHolder c = new CutomPlaceHolder();
            c.MyTypes = new CustomType(8);


            CutomPlaceHolder cc = new CutomPlaceHolder();
            cc.MyTypes = new CustomType(17);

            CutomPlaceHolder ccc = new CutomPlaceHolder();
            ccc.MyTypes = new CustomType(23);


            List<CutomPlaceHolder> list = new List<CutomPlaceHolder>();
            list.Add(c);
            list.Add(cc);
            list.Add(ccc);


            ObjectQuery<CutomPlaceHolder> oq = new ObjectQuery<CutomPlaceHolder>();
            object k = oq.Select(list, "MyTypes > 17");

            CutomPlaceHolder[] kl = (CutomPlaceHolder[])k;

            Assert.IsTrue(kl[0].MyTypes.X == 23);
        }


        [Test]
        public void TestSelectIComparableSmaller()
        {
            CutomPlaceHolder c = new CutomPlaceHolder();
            c.MyTypes = new CustomType(8);


            CutomPlaceHolder cc = new CutomPlaceHolder();
            cc.MyTypes = new CustomType(17);

            CutomPlaceHolder ccc = new CutomPlaceHolder();
            ccc.MyTypes = new CustomType(23);


            List<CutomPlaceHolder> list = new List<CutomPlaceHolder>();
            list.Add(c);
            list.Add(cc);
            list.Add(ccc);


            ObjectQuery<CutomPlaceHolder> oq = new ObjectQuery<CutomPlaceHolder>();
            object k = oq.Select(list, "MyTypes < 17");

            CutomPlaceHolder[] kl = (CutomPlaceHolder[])k;

            Assert.IsTrue(kl[0].MyTypes.X == 8);
        }


        [Test]
        public void TestSelectIComparableBetweenSimple()
        {
            CutomPlaceHolder c = new CutomPlaceHolder();
            c.MyTypes = new CustomType(8);


            CutomPlaceHolder cc = new CutomPlaceHolder();
            cc.MyTypes = new CustomType(17);

            CutomPlaceHolder ccc = new CutomPlaceHolder();
            ccc.MyTypes = new CustomType(23);


            List<CutomPlaceHolder> list = new List<CutomPlaceHolder>();
            list.Add(c);
            list.Add(cc);
            list.Add(ccc);


            ObjectQuery<CutomPlaceHolder> oq = new ObjectQuery<CutomPlaceHolder>();
            object k = oq.Select(list, "MyTypes between 20 30");

            CutomPlaceHolder[] kl = (CutomPlaceHolder[])k;

            Assert.IsTrue(kl.Length == 1);
        }
        
        #endregion

        #endregion

        #region complex
        [Test]
        public void TestComplex()
        {
            Process[] proc = Process.GetProcesses();

            ObjectQuery<Process> q = new ObjectQuery<Process>();
            Process[] pc = (Process[]) q.Select(proc, "BasePriority > 3 and Responding = true and MainWindowTitle like C%");

            Console.WriteLine("SelectComplex");
            foreach (Process var in pc)
            {
                Console.WriteLine(var.MainWindowTitle);
            }
        }


        [Test]
        public void TestComplex2()
        {
            List<TestStructure> list = this.GetTestData();

            ObjectQuery<TestStructure> q = new ObjectQuery<TestStructure>();
            TestStructure[] k = (TestStructure[]) q.Select(list, "Age > 23 and IsMale = true and Name <> Marica and BirthDate <> 1950/7/12");

            Console.WriteLine("TestComplex2");

            foreach (TestStructure var in k)
            {
                Console.WriteLine(var.Name);
            }

        }


        [Test]
        public void TestParserComplex()
        {
            List<TestStructure> list = this.GetTestData();

            ObjectQuery<TestStructure> q = new ObjectQuery<TestStructure>();
            TestStructure[] k = (TestStructure[]) q.Select(list, "Age > 23 or Name like Oooof% and IsMale = true and Name <> Marica and BirthDate <> [1950,7,12] or IsMale = false or Name = [Gogu Pitulice or Goguonavu] or BirthDate between [2000,1,1] [2003,10,12] and CType > 34");

            Console.WriteLine("TestParserComplex");

            foreach (TestStructure var in k)
            {
                Console.WriteLine(var.Name);
            }

        }

        #endregion


        #region "AVG"
        [Test]
        public void TestAvg()
        {
            List<TestStructure> list = this.GetTestData();

            ObjectQuery<TestStructure> q = new ObjectQuery<TestStructure>();
            decimal k = Convert.ToDecimal(q.Avg(list, "Age"));
                
            Console.WriteLine("TestAvg");

            Assert.IsTrue(k > 0);
        }
         #endregion


        #region Sum
        [Test]
        public void TestSum()
        {
            List<TestStructure> list = this.GetTestData();

            ObjectQuery<TestStructure> q = new ObjectQuery<TestStructure>();
            decimal k = Convert.ToDecimal(q.Sum(list, "Age"));

            Console.WriteLine("TestSum");

            Assert.IsTrue(k > 0);
        }


        [Test]
        public void TestSumCondition()
        {
            List<TestStructure> list = this.GetTestData();

            ObjectQuery<TestStructure> q = new ObjectQuery<TestStructure>();
            decimal k = Convert.ToDecimal(q.Sum(list, "Age", "Age > 3"));

            Console.WriteLine("TestSum");

            Assert.IsTrue(k > 0);
        }
        #endregion



    }
}
