using System.Linq;
using UnityEngine;

namespace FortyWorks.SmarTrail
{
    public class CatmullRomDivider
    {
        private readonly WayPoint[] _baseWayPoints;
        private int _subDivision;
        private int _pointToPointDividecount;
        private readonly int _lineCount;

        public CatmullRomDivider(
            WayPoint[] baseWayPoints,
            int subDivision
        )
        {
            _baseWayPoints = baseWayPoints;
            _lineCount = baseWayPoints.Length;
            _subDivision = subDivision;

            CreatePtoPDivideCount();
        }

        private void CreatePtoPDivideCount()
        {
            if (_subDivision % _lineCount != 0)
                _subDivision = _lineCount * Mathf.FloorToInt(_subDivision / _lineCount);

            if (_subDivision <= 0)
                _subDivision = _lineCount;

            _pointToPointDividecount = _subDivision / _lineCount;
        }

        public WayPoint[] SubDivide()
        {
            return Enumerable
                .Range(0, _lineCount)
                .SelectMany(index => DivideLine(index))
                .ToArray();
        }

        private WayPoint[] DivideLine(int divideLineIndex)
        {
            var step = 1f / _pointToPointDividecount;

            return Enumerable.Range(0, _pointToPointDividecount)
                .Select(index =>
                {
                    if (divideLineIndex == 0)
                    {
                        return new WayPoint(
                            CatmullRomFirst(
                                index * step,
                                this._baseWayPoints[divideLineIndex].MidPosition,
                                this._baseWayPoints[divideLineIndex + 1].MidPosition,
                                this._baseWayPoints[divideLineIndex + 2].MidPosition
                            ),
                            Vector3.Lerp(
                                this._baseWayPoints[divideLineIndex].Forward,
                                this._baseWayPoints[divideLineIndex + 2].Forward,
                                index * step),
                            Vector3.Lerp(
                                this._baseWayPoints[divideLineIndex].Right,
                                this._baseWayPoints[divideLineIndex + 1].Right,
                                index * step)
                        );
                    }

                    if (divideLineIndex == _baseWayPoints.Length - 2)
                    {
                        return new WayPoint(
                            CatmullRomLast(
                                index * step,
                                this._baseWayPoints[divideLineIndex - 1].MidPosition,
                                this._baseWayPoints[divideLineIndex].MidPosition,
                                this._baseWayPoints[divideLineIndex + 1].MidPosition
                            ),
                            Vector3.Lerp(
                                this._baseWayPoints[divideLineIndex - 1].Forward,
                                this._baseWayPoints[divideLineIndex + 1].Forward,
                                index * step),
                            Vector3.Lerp(
                                this._baseWayPoints[divideLineIndex].Right,
                                this._baseWayPoints[divideLineIndex + 1].Right,
                                index * step)
                        );
                    }

                    if (divideLineIndex == _baseWayPoints.Length - 1)
                        return this._baseWayPoints[divideLineIndex];

                    return new WayPoint(
                        CatmullRomCommon(
                            index * step,
                            this._baseWayPoints[divideLineIndex - 1].MidPosition,
                            this._baseWayPoints[divideLineIndex].MidPosition,
                            this._baseWayPoints[(divideLineIndex + 1) % this._baseWayPoints.Length].MidPosition,
                            this._baseWayPoints[(divideLineIndex + 2) % this._baseWayPoints.Length].MidPosition
                        ),
                        Vector3.Lerp(
                                this._baseWayPoints[divideLineIndex].Forward,
                                this._baseWayPoints[divideLineIndex + 1].Forward,
                                index * step),
                            Vector3.Lerp(
                                this._baseWayPoints[divideLineIndex].Right,
                                this._baseWayPoints[divideLineIndex + 1].Right,
                                index * step)
                    );
                }).ToArray();
        }

        private static Vector3 CatmullRomCommon(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            Vector3 a = -p0 + 3f * p1 - 3f * p2 + p3;
            Vector3 b = 2f * p0 - 5f * p1 + 4f * p2 - p3;
            Vector3 c = -p0 + p2;
            Vector3 d = 2f * p1;

            return 0.5f * ((a * t * t * t) + (b * t * t) + (c * t) + d);
        }

        private static Vector3 CatmullRomFirst(float t, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            Vector3 b = p0 - 2f * p1 + p2;
            Vector3 c = -3f * p0 + 4f * p1 - p2;
            Vector3 d = 2f * p0;
            return 0.5f * ((b * t * t) + (c * t) + d);
        }

        private static Vector3 CatmullRomLast(float t, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            Vector3 b = p0 - 2f * p1 + p2;
            Vector3 c = -p0 + p2;
            Vector3 d = 2f * p1;

            return 0.5f * ((b * t * t) + (c * t) + d);
        }
    }
}
