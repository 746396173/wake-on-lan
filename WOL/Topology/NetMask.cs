﻿using System.Text;
using System.Collections;

namespace System.Net.Topology
{
    /// <summary>Represents an IPv4 net mask.</summary>
    public sealed class NetMask : INetMask, IEquatable<NetMask>
    {
        private byte[] _bits;
        internal const int MaskLength = 4;

        private static readonly NetMask _empty = new NetMask();

        /// <summary>Represents an empty IPv4 NetMask (all bits set to 0).</summary>
        public static NetMask Empty { get { return _empty; } }

        /// <summary>Gets the length of the net mask in bits.</summary>
        public int AddressLength { get { return MaskLength * 8; } }

        /// <summary>Gets the amount of set bits from the left side (used in CIDR-Notation of net masks).</summary>
        public int Cidr
        {
            get
            {
                System.Diagnostics.Debug.Assert(_bits.Length == MaskLength);
                return _bits.CountFromLeft(true);
            }
        }

        #region Ctors

        /// <summary>Creates a new instance of <see cref="T:System.Net.Topology.NetMask"/> with all bits set to 0.</summary>
        public NetMask()
        {
            _bits = new byte[MaskLength];
        }

        /// <summary>Creates a new instance of <see cref="T:System.Net.Topology.NetMask"/> from an array of <see cref="System.Byte"/>.</summary>
        public NetMask(byte[] value)
        {
            if (value == null || value.Length == 0)
            {
                // maybe throw ArgumentNullException?
                _bits = new byte[MaskLength];
                return;
            }

            if (value.Length != MaskLength)
                throw new ArgumentException("Invalid mask length.");

            _bits = new byte[] { value[0], value[1], value[2], value[3] }; //new BitArray(value); // The BitArray screws things up, need to change to byte[4].
        }

        /// <summary>Creates a new instance of <see cref="T:System.Net.Topology.NetMask"/> from a given <see cref="T:System.Net.IPAddress"/>.</summary>
        /// <param name="address">The IPv4 address.</param>
        public NetMask(IPAddress address)
            : this(address == null ? null : address.GetAddressBytes())
        { }

        /// <summary>Creates a new instance of <see cref="T:System.Net.Topology.NetMask"/>.</summary>
        /// <param name="m1">The first byte.</param>
        /// <param name="m2">The second byte.</param>
        /// <param name="m3">The third byte.</param>
        /// <param name="m4">The fourth byte.</param>
        public NetMask(byte m1, byte m2, byte m3, byte m4)
            : this(new byte[MaskLength] { m1, m2, m3, m4 })
        { }

        /// <summary>Creates a new instance of <see cref="T:System.Net.Topology.NetMask"/>.</summary>
        /// <param name="mask">The mask represented by a 32-Bit integer.</param>
        public NetMask(int mask) // uint is not CLS-compliant, so int will do the job.
        {
            var bytes = BitConverter.GetBytes(mask);
            _bits = new byte[] { bytes[3], bytes[2], bytes[1], bytes[0] }; // TODO: Testing
        }

        /*
         * TODO: Reimplement this constructor
        /// <summary>Creates a new instance of <see cref="T:System.Net.Topology.NetMask"/> using a <see cref="T:System.Collections.BitArray"/>.</summary>
        /// <param name="mask">The mask represented by a <see cref="T:System.Collections.BitArray"/>.</param>
        public NetMask(BitArray mask)
        {
            if (mask == null)
                throw new ArgumentNullException("mask");
            if (mask.Count != 32)
                throw new ArgumentException("Invalid mask length.");

            _bits = new BitArray(mask);
        }
        */

        #endregion

        /// <summary>Gets the bits of the net mask instance as an BitArray object instance.</summary>
        /// <returns>The bits of the net mask instance as an BitArray object instance.</returns>
        public byte[] GetMaskBytes()
        {
            return new byte[] { _bits[0], _bits[1], _bits[2], _bits[3] };
        }

        public static bool IsValidNetMask(byte[] bytes)
        {
            return bytes.RepresentsValidNetMask();
        }

        #region Operators

        #region Equality

        /// <summary>Returns a other indicating whether two instances of <see cref="T:System.Net.Topology.NetMask" /> are equal.</summary>
        /// <param name="n1">The first other to compare.</param>
        /// <param name="n2">The second other to compare.</param>
        /// <returns>true if <paramref name="n1" /> and <paramref name="n2" /> are equal; otherwise, false.</returns>
        public static bool operator ==(NetMask n1, NetMask n2)
        {
            if (Object.ReferenceEquals(n1, n2))
                return true;
            if (((object)n1 == null) || ((object)n2 == null))
                return false;
            return n1.Equals(n2);
        }

        /// <summary>Returns a other indicating whether two instances of <see cref="T:System.Net.Topology.NetMask" /> are not equal.</summary>
        /// <param name="n1">The first other to compare. </param>
        /// <param name="n2">The second other to compare. </param>
        /// <returns>true if <paramref name="n1" /> and <paramref name="n2" /> are not equal; otherwise, false.</returns>
        public static bool operator !=(NetMask n1, NetMask n2)
        {
            return !(n1 == n2); // Problem solved
        }

        #endregion
        #region And

        /// <summary>Bitwise combines a <see cref="T:System.Net.Topology.NetMask" /> instance and an <see cref="T:System.Net.IPAddress"/> the AND operation.</summary>
        /// <param name="mask">The net mask.</param>
        /// <param name="address">The IPAddress.</param>
        /// <returns>The bitwised combination using the AND operation.</returns>
        public static IPAddress operator &(IPAddress address, NetMask mask)
        {
            return mask & address;
        }

        /// <summary>Bitwise combines a <see cref="T:System.Net.Topology.NetMask" /> instance and an <see cref="T:System.Net.IPAddress"/> the AND operation.</summary>
        /// <param name="mask">The net mask.</param>
        /// <param name="address">The IPAddress.</param>
        /// <returns>The bitwised combination using the AND operation.</returns>
        public static IPAddress BitwiseAnd(IPAddress address, NetMask mask)
        {
            return mask & address;
        }

        /// <summary>Bitwise combines a <see cref="T:System.Net.Topology.NetMask" /> instance and an <see cref="T:System.Net.IPAddress"/> the AND operation.</summary>
        /// <param name="mask">The net mask.</param>
        /// <param name="address">The IPAddress.</param>
        /// <returns>The bitwised combination using the AND operation.</returns>
        public static IPAddress operator &(NetMask mask, IPAddress address)
        {
            throw new NotImplementedException();
        }

        /// <summary>Bitwise combines a <see cref="T:System.Net.Topology.NetMask" /> instance and an <see cref="T:System.Net.IPAddress"/> the AND operation.</summary>
        /// <param name="mask">The net mask.</param>
        /// <param name="address">The IPAddress.</param>
        /// <returns>The bitwised combination using the AND operation.</returns>
        public static IPAddress BitwiseAnd(NetMask mask, IPAddress address)
        {
            return mask & address;
        }

        /// <summary>Bitwise combines the two instances of <see cref="T:System.Net.Topology.NetMask" /> using the AND operation.</summary>
        /// <param name="n1">The first other.</param>
        /// <param name="n2">The second other.</param>
        /// <returns>The bitwised combination using the AND operation.</returns>
        public static NetMask operator &(NetMask n1, NetMask n2)
        {
            if (n1 == null || n2 == null)
                return NetMask.Empty;
            return new NetMask(n1._bits.And(n2._bits));
        }

        /// <summary>Bitwise combines the two instances of <see cref="T:System.Net.Topology.NetMask" /> using the AND operation.</summary>
        /// <param name="n1">The first other.</param>
        /// <param name="n2">The second other.</param>
        /// <returns>The bitwised combination using the AND operation.</returns>
        public static NetMask BitwiseAnd(NetMask n1, NetMask n2)
        {
            return n1 & n2;
        }

        #endregion
        #region Or

        /// <summary>Bitwise combines the two instances of <see cref="T:System.Net.Topology.NetMask" /> using the OR operation.</summary>
        /// <param name="n1">The first other.</param>
        /// <param name="n2">The second other.</param>
        /// <returns>The bitwised combination using the OR operation.</returns>
        public static NetMask operator |(NetMask n1, NetMask n2)
        {
            if (n1 == null)
                return n2 == null ? NetMask.Empty : n2;
            if (n2 == null)
                return n1;
            return new NetMask(n1._bits.Or(n2._bits));
        }

        /// <summary>Bitwise combines the two instances of <see cref="T:System.Net.Topology.NetMask" /> using the OR operation.</summary>
        /// <param name="n1">The first other.</param>
        /// <param name="n2">The second other.</param>
        /// <returns>The bitwised combination using the OR operation.</returns>
        public static NetMask BitwiseOr(NetMask n1, NetMask n2)
        {
            return n1 | n2;
        }

        #endregion

        #endregion
        #region Common overrides

        /// <summary>Converts the other of this instance to its equivalent string representation.</summary>
        /// <returns>A string that represents the other of this instance.</returns>
        /// <filterpriority>1</filterpriority>
        public override string ToString()
        {
            var sb = new StringBuilder(4 * 3 + 3 * 3 + 32 + 3 + 3); // 255.255.255.255 (11111111111111111111111111111111)

            var arr = new byte[4];
            _bits.CopyTo(arr, 0);
            var asString = _bits.ToBinaryString('.');

            sb.Append(arr[0]).Append('.').Append(arr[1]).Append('.').Append(arr[2]).Append('.').Append(arr[3]).Append(" (");
            sb.Append(asString).Append(')');

            return sb.ToString();
        }

        /// <summary>Returns a other indicating whether this instance and a specified <see cref="T:System.Object" /> represent the same type and other.</summary>
        /// <returns>true if <paramref name="obj" /> is a <see cref="T:System.Net.Topology.NetMask" /> and equal to this instance; otherwise, false.</returns>
        /// <param name="obj">The object to compare with this instance. </param>
        /// <filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var mask = obj as NetMask;
            if ((object)mask == null)
                return false;

            return Equals(mask);
        }

        /// <summary>Returns a other indicating whether this instance and a specified <see cref="T:System.Net.Topology.NetMask" /> object represent the same other.</summary>
        /// <returns>true if <paramref name="other" /> is equal to this instance; otherwise, false.</returns>
        /// <param name="other">An object to compare to this instance.</param>
        /// <filterpriority>2</filterpriority>
        public bool Equals(NetMask other)
        {
            if (other == null)
                return false;

            if (other._bits.Length != MaskLength)
                return false;
            if (other._bits.Length != _bits.Length)
                return false;

            /*
            // More universal approach:
            for (int i = 0; i < _bits.Length; ++i)
                if (_bits[i] != other._bits[i])
                    return false;
            return true;
            */
            
            // faster approach:
            return _bits[0] == other._bits[0]
                && _bits[1] == other._bits[1]
                && _bits[2] == other._bits[2]
                && _bits[3] == other._bits[3];
        }

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return _bits.GetHashCode();
        }

        #endregion
    }
}
