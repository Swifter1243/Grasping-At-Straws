// --- https://www.shadertoy.com/view/ldl3Dl
float3 hash( float3 x )
{
    x = float3( dot(x,float3(127.1,311.7, 74.7)),
              dot(x,float3(269.5,183.3,246.1)),
              dot(x,float3(113.5,271.9,124.6)));

    return frac(sin(x)*43758.5453123);
}
float3 voronoi( in float3 x )
{
    float3 p = floor( x );
    float3 f = frac( x );

    float id = 0.0;
    float2 res = 100;
    for( int k=-1; k<=1; k++ )
        for( int j=-1; j<=1; j++ )
            for( int i=-1; i<=1; i++ )
            {
                float3 b = float3( float(i), float(j), float(k) );
                float3 r = float3( b ) - f + hash( p + b );
                float d = dot( r, r );

                if( d < res.x )
                {
                    id = dot( p+b, float3(1.0,57.0,113.0 ) );
                    res = float2( d, res.x );
                }
                else if( d < res.y )
                {
                    res.y = d;
                }
            }

    return float3( sqrt( res ), abs(id) );
}