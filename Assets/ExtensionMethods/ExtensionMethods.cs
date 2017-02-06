using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

/*
Carson's extension methods for C#/Unity
*/

public static class NumberExtensionMethods {
    public static double MinMax( this double number, double min, double max ) {
        if ( number < min ) {
            return min;
        }
        if ( number > max ) {
            return max;
        }
        return number;
    }

    public static float MinMax( this float number, float min, float max ) {
        return (float)MinMax( (double)number, (double)min, (double)max );
    }

    public static int MinMax( this int number, int min, int max ) {
        return (int)MinMax( (double)number, (double)min, (double)max );
    }
}

public static class GameObjectExtensionMethods {
    public static bool HasComponent<T>( this GameObject gameObject ) where T : Component {
        return gameObject.GetComponent<T>() != null;
    }

    public static float GetAnglesOffFromFacing(this GameObject self, GameObject other) {
        var S = self.transform.position.DropY();
        var Sf = (self.transform.position + self.transform.forward).DropY();
        var So = (self.transform.position + (other.transform.position - self.transform.position).normalized).DropY();

        var StoSf = Vector2.Distance( S, Sf );
        var SftoSo = Vector2.Distance( Sf, So );

        if (StoSf != 0 ) {
            return Mathf.Rad2Deg * Mathf.Atan( SftoSo / StoSf );
        }
        return 0f;
    }
}

public static class TransformExtensionMethods {
    public static void SetZRotation( this Transform transform, float zDegrees ) {
        transform.eulerAngles = new Vector3( transform.eulerAngles.x, transform.eulerAngles.z, zDegrees % 360 );
    }

    public static void SetYRotation( this Transform transform, float yDegrees ) {
        transform.eulerAngles = new Vector3( transform.eulerAngles.x, yDegrees % 360, transform.eulerAngles.z );
    }

    public static void SetXRotation( this Transform transform, float xDegrees ) {
        transform.eulerAngles = new Vector3( xDegrees % 360, transform.eulerAngles.y, transform.eulerAngles.z );
    }

    public static void SetRotation( this Transform transform, float xDegrees, float yDegrees, float zDegrees ) {
        transform.eulerAngles = new Vector3( xDegrees % 360, yDegrees % 360, zDegrees % 360 );
    }

    public static void SetRelativeRotation( this Transform transform, float xDegrees, float yDegrees, float zDegrees ) {
        var oldX = transform.eulerAngles.x;
        var oldY = transform.eulerAngles.y;
        var oldZ = transform.eulerAngles.z;
        var newXDegree = oldX + xDegrees;
        var newYDegree = oldY + yDegrees;
        var newZDegree = oldZ + zDegrees;
        if ( newXDegree < 0 )
            newXDegree = 360 - newXDegree;
        if ( newYDegree < 0 )
            newYDegree = 360 - newYDegree;
        if ( newZDegree < 0 )
            newZDegree = 360 - newZDegree;
        transform.eulerAngles = new Vector3( (newXDegree) % 360, (newYDegree) % 360, (newZDegree) % 360 );
    }

    public static void SetRelativeXRotation( this Transform transform, float xDegrees ) {
        var oldX = transform.eulerAngles.x;
        var newDegrees = oldX + xDegrees;
        if ( newDegrees < 0 )
            newDegrees = 360 - newDegrees;
        transform.eulerAngles = new Vector3( (newDegrees) % 360, transform.eulerAngles.y, transform.eulerAngles.z );
    }

    public static void SetRelativeYRotation( this Transform transform, float yDegrees ) {
        var oldY = transform.eulerAngles.y;
        var newDegrees = oldY + yDegrees;
        if ( newDegrees < 0 )
            newDegrees = 360 - newDegrees;
        transform.eulerAngles = new Vector3( transform.eulerAngles.x, (newDegrees) % 360, transform.eulerAngles.z );
    }

    public static void SetRelativeZRotation( this Transform transform, float zDegrees ) {
        var oldZ = transform.eulerAngles.z;
        var newDegrees = oldZ + zDegrees;
        if ( newDegrees < 0 )
            newDegrees = 360 - newDegrees;
        transform.eulerAngles = new Vector3( transform.eulerAngles.x, transform.eulerAngles.y, (newDegrees) % 360 );
    }

    public static float LookAngleOffset( this Transform transform, Vector3 idealNormalizedDirection ) {
        Vector3 perp = Vector3.Cross( transform.forward, idealNormalizedDirection );
        return Vector3.Dot( perp, transform.up );
    }

    public static Vector3 RelativeDirectionOf(this Transform transform, GameObject @object) {
        return transform.RelativeDirectionOf( @object.transform.position );
    }

    public static Vector3 RelativeDirectionOf(this Transform transform, Transform @object) {
        return transform.RelativeDirectionOf( @object.position );
    }

    public static Vector3 RelativeDirectionOf(this Transform transform, Vector3 point) {
        return (point - transform.position).normalized;
    }
}

public static class VectorExtensionMethods {
    public static float AngleBetween( this Vector2 vec1, Vector2 vec2 ) {
        Vector2 diference = vec2 - vec1;
        float sign = (vec2.y < vec1.y) ? -1.0f : 1.0f;
        return Vector2.Angle( Vector2.right, diference ) * sign;
    }

    //Unfinished/tested
    public static float AngleBetween( this Vector3 vec1, Vector3 vec2 ) {
        var diference = vec2 - vec1;
        var sign = (vec2.y < vec1.y) ? -1.0f : 1.0f;
        var angle = Vector3.Angle( Vector3.right, diference ) * sign;
        Debug.Log( angle + " " + sign );
        return angle;
    }

    public static Vector2 xy( this Vector3 v ) {
        return new Vector2( v.x, v.y );
    }

    public static Vector3 WithX( this Vector3 v, float x ) {
        return new Vector3( x, v.y, v.z );
    }

    public static Vector3 WithY( this Vector3 v, float y ) {
        return new Vector3( v.x, y, v.z );
    }

    public static Vector3 WithZ( this Vector3 v, float z ) {
        return new Vector3( v.x, v.y, z );
    }

    public static Vector2 WithX( this Vector2 v, float x ) {
        return new Vector2( x, v.y );
    }

    public static Vector2 WithY( this Vector2 v, float y ) {
        return new Vector2( v.x, y );
    }

    public static Vector3 WithZ( this Vector2 v, float z ) {
        return new Vector3( v.x, v.y, z );
    }

    public static float DistanceToNoY( this Vector3 v1, Vector3 v2 ) {
        return Vector3.Distance( v1.WithY( 0 ), v2.WithY( 0 ) );
    }

    public static Vector2 DropX(this Vector3 v) {
        return new Vector2( v.y, v.z );
    }

    public static Vector2 DropY( this Vector3 v ) {
        return new Vector2( v.x, v.z );
    }

    public static Vector2 DropZ( this Vector3 v ) {
        return new Vector2( v.x, v.y );
    }
}

public static class ListExtensionMethods {
    public static void Shuffle<T>( this IList<T> list ) {
        System.Random rng = new System.Random();
        int n = list.Count;
        while ( n > 1 ) {
            n--;
            int k = rng.Next( n + 1 );
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static T RandomItem<T>( this IList<T> list ) {
        if ( list.Count == 0 )
            throw new System.IndexOutOfRangeException( "Cannot select a random item from an empty list" );
        return list[UnityEngine.Random.Range( 0, list.Count )];
    }

    public static T RemoveRandom<T>( this IList<T> list ) {
        if ( list.Count == 0 )
            throw new System.IndexOutOfRangeException( "Cannot remove a random item from an empty list" );
        int index = UnityEngine.Random.Range( 0, list.Count );
        T item = list[index];
        list.RemoveAt( index );
        return item;
    }

    public static List<T> With<T>( this List<T> list, params T[] prams ) {
        foreach ( var param in prams ) {
            list.Add( param );
        }
        return list;
    }
}

public static class StringExtensionMethods {
    public static string Truncate( this string value, int maxLength ) {
        if ( string.IsNullOrEmpty( value ) )
            return value;
        return value.Length <= maxLength ? value : value.Substring( 0, maxLength );
    }

    public static bool IsNullOrEmpty( this string value ) {
        return string.IsNullOrEmpty( value );
    }
}


