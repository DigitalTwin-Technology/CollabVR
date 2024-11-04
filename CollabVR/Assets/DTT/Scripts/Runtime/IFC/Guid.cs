using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace BimViz
{

public static class Guid
{
    private static string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_$";

    #region Public

    public static string Compress( string g )
    {
        List < int > bs = new List < int >();

        for ( int i = 0; i < g.Length; i += 2 )
        {
            bs.Add( int.Parse( g.Substring( i, 2 ), NumberStyles.HexNumber ) );
        }

        string B64( int v, int l = 4 )
        {
            StringBuilder sb = new StringBuilder();

            for ( int i = l - 1; i >= 0; i-- )
            {
                sb.Append( chars[v / ( int )Math.Pow( 64, i ) % 64] );
            }

            return sb.ToString();
        }

        List < string > parts = new List < string >();
        parts.Add( B64( bs[0], 2 ) );

        for ( int i = 1; i < 16; i += 3 )
        {
            parts.Add( B64( ( bs[i] << 16 ) + ( bs[i + 1] << 8 ) + bs[i + 2] ) );
        }

        return string.Join( "", parts );
    }

    public static string Expand( string g )
    {
        Func < string, int > b64 = ( v ) =>
        {
            int result = 0;

            for ( int i = 0; i < v.Length; i++ )
            {
                result += chars.IndexOf( v[i] ) * ( int )Math.Pow( 64, v.Length - i - 1 );
            }

            return result;
        };

        List < byte > bs = new List < byte >();
        bs.Add( ( byte )b64( g.Substring( 0, 2 ) ) );

        for ( int i = 0; i < 5; i++ )
        {
            int d = b64( g.Substring( 2 + 4 * i, 4 ) );
            bs.Add( ( byte )( ( d >> 16 ) & 0xFF ) );
            bs.Add( ( byte )( ( d >> 8 ) & 0xFF ) );
            bs.Add( ( byte )( d & 0xFF ) );
        }

        return BitConverter.ToString( bs.ToArray() ).Replace( "-", "" ).ToLower();
    }

    public static string ExpandAndSplit( string g )
    {
        return Split( Expand( g ) );
    }

    public static string GetLongId( string id )
    {
        if ( IsLong( id ) )
        {
            return id;
        }

        return ExpandAndSplit( id );
    }

    public static bool IsLong( string g )
    {
        if ( g.Length != 36 )
        {
            return false;
        }

        string[] split = g.Split( '-' );

        return split.Length == 5 &&
               split[0].Length == 8 &&
               split[1].Length == 4 &&
               split[2].Length == 4 &&
               split[3].Length == 4 &&
               split[4].Length == 12;
    }

    public static string Merge( string guid )
    {
        return guid.Replace( "-", "" );
    }

    public static string MergeAndCompress( string guid )
    {
        return Compress( Merge( guid ) );
    }

    public static string Split( string g )
    {
        return /*"{" +*/
            g.Substring( 0, 8 ) +
            "-" +
            g.Substring( 8, 4 ) +
            "-" +
            g.Substring( 12, 4 ) +
            "-" +
            g.Substring( 16, 4 ) +
            "-" +
            g.Substring( 20, 12 ) /* + "}"*/;
    }

    #endregion
}

}
