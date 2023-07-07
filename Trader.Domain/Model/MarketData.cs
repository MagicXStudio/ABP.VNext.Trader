using System;

namespace Trader.Domain.Model
{
    public class DirectoryDetail : IEquatable<DirectoryDetail>
    {
        public DirectoryDetail(string name, string drive, decimal bid, decimal offer)
        {
            Name = name;
            Instrument = drive;
            Bid = bid;
            Offer = offer;
        }


        public string Name { get; set; }
        public string Instrument { get; }
        public decimal Bid { get; }
        public decimal Offer { get; }

        #region Equality

        public bool Equals(DirectoryDetail other)
        {
            return string.Equals(Instrument, other.Instrument) && Bid == other.Bid && Offer == other.Offer;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is DirectoryDetail && Equals((DirectoryDetail)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Instrument != null ? Instrument.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Bid.GetHashCode();
                hashCode = (hashCode * 397) ^ Offer.GetHashCode();
                return hashCode;
            }
        }


        public static DirectoryDetail operator +(DirectoryDetail left, decimal pipsValue)
        {
            var bid = left.Bid + pipsValue;
            var offer = left.Offer + pipsValue;
            return new DirectoryDetail(left.Name, left.Instrument, bid, offer);
        }

        public static DirectoryDetail operator -(DirectoryDetail left, decimal pipsValue)
        {
            var bid = left.Bid - pipsValue;
            var offer = left.Offer - pipsValue;
            return new DirectoryDetail(left.Name, left.Instrument, bid, offer);
        }

        public static bool operator >=(DirectoryDetail left, DirectoryDetail right)
        {
            return left.Bid >= right.Bid;
        }

        public static bool operator <=(DirectoryDetail left, DirectoryDetail right)
        {
            return left.Bid <= right.Bid;
        }

        public static bool operator >(DirectoryDetail left, DirectoryDetail right)
        {
            return left.Bid > right.Bid;
        }

        public static bool operator <(DirectoryDetail left, DirectoryDetail right)
        {
            return left.Bid < right.Bid;
        }

        public static bool operator ==(DirectoryDetail left, DirectoryDetail right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DirectoryDetail left, DirectoryDetail right)
        {
            return !left.Equals(right);
        }

        #endregion

        public override string ToString()
        {
            return $"{Instrument}, {Bid}/{Offer}";
        }
    }
}