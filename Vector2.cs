using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public struct Vector2 {
    public static readonly Vector2 zero = new Vector2(0, 0);
    public static readonly Vector2 right = new Vector2(1, 0);
    public static readonly Vector2 left = new Vector2(-1, 0);
    public static readonly Vector2 forward = new Vector2(0, 1);
    public static readonly Vector2 back = new Vector2(0, -1);

    public float x;
    public float y;

    public Vector2(Vector2 other) {
        x = other.x;
        y = other.y;
    }

    public Vector2(float x, float y) {
        this.x = x;
        this.y = y;
    }

    public static Vector2 operator +(Vector2 v1, Vector2 v2) {
        Vector2 vector = new Vector2(v1.x + v2.x, v1.y + v2.y);
        return vector;
    }

    public static Vector2 operator -(Vector2 v1, Vector2 v2) {
        return new Vector2(v1.x - v2.x, v1.y - v2.y);
    }

    public static Vector2 operator /(Vector2 v1, float c) {
        return new Vector2(v1.x / c, v1.y / c);
    }

    public static Vector2 operator *(Vector2 v1, float c) {
        return new Vector2(v1.x * c, v1.y * c);
    }

    public static Vector2 operator *(float c, Vector2 v1) {
        return new Vector2(v1.x * c, v1.y * c);
    }

    public static float operator *(Vector2 v1, Vector2 v2) {
        return v1.x * v2.x + v1.y * v2.y;
    }

    public static bool operator ==(Vector2 v1, Vector2 v2) {
        return (v1.x == v2.x && v1.y == v2.y);
    }

    public static bool operator !=(Vector2 v1, Vector2 v2) {
        return (v1.x != v2.x || v1.y != v2.y);
    }

    public float Magnitude() {
        float magnitude = (float)Math.Sqrt((x * x) + (y * y));
        return magnitude;
    }

    public void Normalize() {
        float magnitude = Magnitude();
        if (magnitude != 0) {
            x /= magnitude;
            y /= magnitude;
        }
    }

    public Vector2 Normalized() {
        Vector2 normalized = new Vector2(x, y);
        normalized.Normalize();
        return normalized;
    }

    public float Distance(Vector2 other) {
        float dx = (x - other.x) * (x - other.x);
        float dy = (y - other.y) * (y - other.y);
        float distance = (float)Math.Sqrt(dx + dy);
        return distance;
    }

    public override string ToString() {
        return "(" + x + ", " + y + ")";
    }

    public void Rotate(Vector2 pivot, float rotation) {
        Vector2 v = this - pivot;
        x = (float)(v.x * Math.Cos(rotation) - v.y * Math.Sin(rotation) + pivot.x);
        y = (float)(v.x * Math.Sin(rotation) + v.y * Math.Cos(rotation) + pivot.y);
    }
}