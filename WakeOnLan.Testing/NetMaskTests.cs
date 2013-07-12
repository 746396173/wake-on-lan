﻿using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Net.Topology;

namespace WakeOnLan.Testing
{
    [TestClass]
    public class NetMaskTests : TestHelper
    {
        [TestMethod]
        public void Constructor()
        {
            var ip = IPAddress.Parse("255.255.248.0");
            var expected = new NetMask(255, 255, 248, 0);
            var actual = new NetMask(ip);
            Assert.AreEqual(expected, actual);

            var m1 = new NetMask(255, 255, 248, 0);
            var m2 = new NetMask(-2048);
            Assert.IsTrue(m1 == m2);

            var m3 = new NetMask(Ba( 255, 255, 248, 0 ));
            Assert.IsTrue(m1 == m3);

            var m4 = new NetMask((byte[])null);
            Assert.IsTrue(m4 == NetMask.Empty);

            var m5 = new NetMask(Ba());
            Assert.IsTrue(m5 == NetMask.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        [TestCategory("ArgumentException Tests")]
        public void ConstructorEx()
        {
            var m3 = new NetMask(Ba( 255, 255, 248, 0, 0 ));
        }

        [TestMethod]
        public void EqualityOperator()
        {
            var m1 = new NetMask(255, 255, 248, 0);
            var m2 = new NetMask(255, 255, 248, 0);
            var m3 = new NetMask(255, 255, 0, 0);

            Assert.AreEqual(m1, m2);
            Assert.IsTrue(m1 == m2);
            Assert.IsFalse(m1 == m3);
            Assert.IsTrue(m1 != m3);
        }

        [TestMethod]
        public void Empty()
        {
            Assert.IsTrue(new NetMask(0, 0, 0, 0) == NetMask.Empty);
            Assert.IsTrue(new NetMask(0) == NetMask.Empty);
        }

        [TestMethod]
        public void Length()
        {
            Assert.AreEqual(32, NetMask.MaskLength * 8);
            Assert.AreEqual(32, NetMask.Empty.AddressLength);
        }

        [TestMethod]
        public void Cidr()
        {
            var a = new TestingCollection<byte[], int> {
                new BaITestItem( Ba(0x00, 0x00, 0x00, 0x00), 0),
                new BaITestItem( Ba(0xFF, 0x00, 0x00, 0x00), 8),
                new BaITestItem( Ba(0xFF, 0xFF, 0x00, 0x00), 16),
                new BaITestItem( Ba(0xFF, 0xFF, 0xFF, 0x00), 24),
                new BaITestItem( Ba(0xFF, 0xFF, 0xFF, 0xFF), 32),
                new BaITestItem( Ba(0xFF, 0xFF, 0xFE, 0x00), 23),
                new BaITestItem( Ba(0xFF, 0x80, 0x00, 0x00), 9),
                new BaITestItem( Ba(0xFF, 0xFF, 0x80, 0x00), 17),
                new BaITestItem( Ba(0xFF, 0xFF, 0xFE, 0x00), 23),
                new BaITestItem( Ba(0xFF, 0xFF, 0xFF, 0xF8), 29),
                new BaITestItem( Ba(0xFF, 0xFF, 0xFF, 0xFC), 30)
            };

            foreach (var i in a)
            {
                var nm = new NetMask(i.ToTest1);
                int cidr = nm.Cidr;
                Assert.AreEqual(i.Expected, cidr);
            }
        }

        [TestMethod]
        public void ToStringTest()
        {
            //var m1 = new NetMask(255, 255, 248, 0);
            var m1 = new NetMask(-2048);
            var expected = "255.255.248.0 (11111111.11111111.11111000.00000000)";
            var str = m1.ToString();
            Assert.AreEqual(expected, str);
        }

        [TestMethod]
        public void GetMaskBytes()
        {
            var actual = Ba(255, 255, 248, 0);
            var m = new NetMask(actual);
            var expected = m.GetMaskBytes();
            Assert.IsTrue(actual.SequenceEqual(expected));
        }

        [TestMethod]
        public void EqualsImplementation()
        {
            int a = 2;
            var ip = new IPAddress(0);
            var m = new NetMask(255, 255, 255, 0);
            var m1 = new NetMask(255, 255, 255, 0);

            Assert.IsFalse(m.Equals(a));
            Assert.IsFalse(m.Equals(ip));
            Assert.IsFalse(m.Equals(null));
            Assert.IsFalse(m.Equals((object)null));

            Assert.IsTrue(m.Equals(m));
            Assert.IsTrue(m.Equals(m1));
            Assert.IsTrue(m1.Equals(m));
        }

        [TestMethod]
        public void Or()
        {
            var m1 = new NetMask(255, 255, 248, 0);
            var m2 = new NetMask(255, 255, 0, 0);

            var mOr = m1 | m2;
            Assert.AreEqual(m1, mOr);

            mOr = NetMask.BitwiseOr(m1, m2);
            Assert.AreEqual(m1, mOr);

            mOr = NetMask.BitwiseOr(m2, m1);
            Assert.AreEqual(m1, mOr);

            mOr = NetMask.BitwiseOr(m2, null);
            Assert.AreEqual(m2, mOr);

            mOr = NetMask.BitwiseOr(null, m1);
            Assert.AreEqual(m1, mOr);
        }

        [TestMethod]
        public void AndMask()
        {
            var m1 = new NetMask(255, 255, 255, 0);
            var m2 = new NetMask(255, 255, 0, 0);

            var mAnd = m1 & m2;
            Assert.AreEqual(m2, mAnd);

            mAnd = m2 & m1;
            Assert.AreEqual(m2, mAnd);
            
            mAnd = NetMask.BitwiseAnd(m1, m2);
            Assert.AreEqual(m2, mAnd);

            mAnd = NetMask.BitwiseAnd(m2, m1);
            Assert.AreEqual(m2, mAnd);
            
            mAnd = NetMask.BitwiseAnd(m2, m2);
            Assert.AreEqual(m2, mAnd);

            mAnd = NetMask.BitwiseAnd(m1, m1);
            Assert.AreEqual(m1, mAnd);


            mAnd = NetMask.BitwiseAnd((NetMask)null, m1);
            Assert.AreEqual(NetMask.Empty, mAnd);

            mAnd = NetMask.BitwiseAnd(m2, (NetMask)null);
            Assert.AreEqual(NetMask.Empty, mAnd);
        }

        [TestMethod]
        public void MaskValidity()
        {
            var b1 = Ba(255, 255, 255, 255);
            var valid = NetMask.IsValidNetMask(b1);
            Assert.AreEqual(true, valid);

            b1 = Ba(255, 255, 255, 0);
            valid = NetMask.IsValidNetMask(b1);
            Assert.AreEqual(true, valid);

            b1 = Ba(255, 255, 0, 0);
            valid = NetMask.IsValidNetMask(b1);
            Assert.AreEqual(true, valid);

            b1 = Ba(255, 0, 0, 0);
            valid = NetMask.IsValidNetMask(b1);
            Assert.AreEqual(true, valid);

            b1 = Ba(0, 0, 0, 0);
            valid = NetMask.IsValidNetMask(b1);
            Assert.AreEqual(true, valid);

            b1 = Ba(0, 255, 255, 255);
            valid = NetMask.IsValidNetMask(b1);
            Assert.AreEqual(false, valid);

            b1 = Ba(0, 0, 255, 255);
            valid = NetMask.IsValidNetMask(b1);
            Assert.AreEqual(false, valid);

            b1 = Ba(0, 0, 0, 255);
            valid = NetMask.IsValidNetMask(b1);
            Assert.AreEqual(false, valid);

            b1 = Ba(255, 0, 255, 255);
            valid = NetMask.IsValidNetMask(b1);
            Assert.AreEqual(false, valid);

            b1 = Ba(255, 0, 0, 255);
            valid = NetMask.IsValidNetMask(b1);
            Assert.AreEqual(false, valid);
            
            b1 = Ba(255, 255, 0, 255);
            valid = NetMask.IsValidNetMask(b1);
            Assert.AreEqual(false, valid);
        }
    }
}