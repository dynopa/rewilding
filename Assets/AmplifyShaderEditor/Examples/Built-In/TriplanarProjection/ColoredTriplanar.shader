// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ColoredTriplanar"
{
	Properties
	{
		_TopAlbedo1("Top Albedo", 2D) = "white" {}
		_TopNormal1("Top Normal", 2D) = "bump" {}
		_TriplanarAlbedo1("Triplanar Albedo", 2D) = "white" {}
		_TriplanarNormal1("Triplanar Normal", 2D) = "bump" {}
		_Specular1("Specular", Range( 0 , 1)) = 0.02
		_Smoothness1("Smoothness", Range( 0 , 1)) = 0.5
		_CoverageFalloff1("Coverage Falloff", Range( 0.01 , 5)) = 0.5
		_Tiling1("Tiling", Vector) = (1,1,0,0)
		_albeto("albeto", Color) = (0.1123238,0.04223033,0.1226415,0)
		_alpha("alpha", Range( 0 , 1)) = 0.8215393
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#define ASE_TEXTURE_PARAMS(textureName) textureName

		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
		};

		uniform sampler2D _TopNormal1;
		uniform sampler2D _TriplanarNormal1;
		uniform float2 _Tiling1;
		uniform float _CoverageFalloff1;
		uniform float4 _albeto;
		uniform sampler2D _TopAlbedo1;
		uniform sampler2D _TriplanarAlbedo1;
		uniform float _alpha;
		uniform float _Specular1;
		uniform float _Smoothness1;


		inline float3 TriplanarSamplingCNF( sampler2D topTexMap, sampler2D midTexMap, sampler2D botTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )
		{
			float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
			projNormal /= ( projNormal.x + projNormal.y + projNormal.z ) + 0.00001;
			float3 nsign = sign( worldNormal );
			float negProjNormalY = max( 0, projNormal.y * -nsign.y );
			projNormal.y = max( 0, projNormal.y * nsign.y );
			half4 xNorm; half4 yNorm; half4 yNormN; half4 zNorm;
			xNorm = ( tex2D( ASE_TEXTURE_PARAMS( midTexMap ), tiling * worldPos.zy * float2( nsign.x, 1.0 ) ) );
			yNorm = ( tex2D( ASE_TEXTURE_PARAMS( topTexMap ), tiling * worldPos.xz * float2( nsign.y, 1.0 ) ) );
			yNormN = ( tex2D( ASE_TEXTURE_PARAMS( botTexMap ), tiling * worldPos.xz * float2( nsign.y, 1.0 ) ) );
			zNorm = ( tex2D( ASE_TEXTURE_PARAMS( midTexMap ), tiling * worldPos.xy * float2( -nsign.z, 1.0 ) ) );
			xNorm.xyz = half3( UnpackNormal( xNorm ).xy * float2( nsign.x, 1.0 ) + worldNormal.zy, worldNormal.x ).zyx;
			yNorm.xyz = half3( UnpackNormal( yNorm ).xy * float2( nsign.y, 1.0 ) + worldNormal.xz, worldNormal.y ).xzy;
			zNorm.xyz = half3( UnpackNormal( zNorm ).xy * float2( -nsign.z, 1.0 ) + worldNormal.xy, worldNormal.z ).xyz;
			yNormN.xyz = half3( UnpackNormal( yNormN  ).xy * float2( nsign.y, 1.0 ) + worldNormal.xz, worldNormal.y ).xzy;
			return normalize( xNorm.xyz * projNormal.x + yNorm.xyz * projNormal.y + yNormN.xyz * negProjNormalY + zNorm.xyz * projNormal.z );
		}


		inline float4 TriplanarSamplingCF( sampler2D topTexMap, sampler2D midTexMap, sampler2D botTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )
		{
			float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
			projNormal /= ( projNormal.x + projNormal.y + projNormal.z ) + 0.00001;
			float3 nsign = sign( worldNormal );
			float negProjNormalY = max( 0, projNormal.y * -nsign.y );
			projNormal.y = max( 0, projNormal.y * nsign.y );
			half4 xNorm; half4 yNorm; half4 yNormN; half4 zNorm;
			xNorm = ( tex2D( ASE_TEXTURE_PARAMS( midTexMap ), tiling * worldPos.zy * float2( nsign.x, 1.0 ) ) );
			yNorm = ( tex2D( ASE_TEXTURE_PARAMS( topTexMap ), tiling * worldPos.xz * float2( nsign.y, 1.0 ) ) );
			yNormN = ( tex2D( ASE_TEXTURE_PARAMS( botTexMap ), tiling * worldPos.xz * float2( nsign.y, 1.0 ) ) );
			zNorm = ( tex2D( ASE_TEXTURE_PARAMS( midTexMap ), tiling * worldPos.xy * float2( -nsign.z, 1.0 ) ) );
			return xNorm * projNormal.x + yNorm * projNormal.y + yNormN * negProjNormalY + zNorm * projNormal.z;
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_worldTangent = WorldNormalVector( i, float3( 1, 0, 0 ) );
			float3 ase_worldBitangent = WorldNormalVector( i, float3( 0, 1, 0 ) );
			float3x3 ase_worldToTangent = float3x3( ase_worldTangent, ase_worldBitangent, ase_worldNormal );
			float3 triplanar11 = TriplanarSamplingCNF( _TopNormal1, _TriplanarNormal1, _TriplanarNormal1, ase_worldPos, ase_worldNormal, _CoverageFalloff1, _Tiling1, float3( 1,1,1 ), float3(0,0,0) );
			float3 tanTriplanarNormal11 = mul( ase_worldToTangent, triplanar11 );
			o.Normal = tanTriplanarNormal11;
			float4 triplanar8 = TriplanarSamplingCF( _TopAlbedo1, _TriplanarAlbedo1, _TriplanarAlbedo1, ase_worldPos, ase_worldNormal, _CoverageFalloff1, _Tiling1, float3( 1,1,1 ), float3(0,0,0) );
			float4 blendOpSrc12 = _albeto;
			float4 blendOpDest12 = triplanar8;
			float4 lerpBlendMode12 = lerp(blendOpDest12, (( blendOpSrc12 > 0.5 ) ? ( 1.0 - ( 1.0 - 2.0 * ( blendOpSrc12 - 0.5 ) ) * ( 1.0 - blendOpDest12 ) ) : ( 2.0 * blendOpSrc12 * blendOpDest12 ) ),_alpha);
			o.Albedo = ( saturate( lerpBlendMode12 )).xyz;
			o.Metallic = _Specular1;
			o.Smoothness = _Smoothness1;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float4 tSpace0 : TEXCOORD1;
				float4 tSpace1 : TEXCOORD2;
				float4 tSpace2 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17101
0;0;1536;843;2046.225;1377.295;3.505356;True;True
Node;AmplifyShaderEditor.TexturePropertyNode;2;-370.4583,-715.1865;Float;True;Property;_TopAlbedo1;Top Albedo;0;0;Create;True;0;0;False;0;ee2523340e7944c4297a80937a51484c;f1bc415e28d17ad42a1bb8624a8a193a;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.TexturePropertyNode;3;-370.4583,-523.1865;Float;True;Property;_TriplanarAlbedo1;Triplanar Albedo;2;0;Create;True;0;0;False;0;ee2523340e7944c4297a80937a51484c;f1bc415e28d17ad42a1bb8624a8a193a;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.WorldPosInputsNode;4;-354.4583,-331.1866;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;5;-425.358,-156.8866;Float;False;Property;_CoverageFalloff1;Coverage Falloff;6;0;Create;True;0;0;False;0;0.5;0.5;0.01;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;1;-854.5314,-169.7056;Inherit;False;Property;_Tiling1;Tiling;7;0;Create;True;0;0;False;0;1,1;0.02,0.02;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TexturePropertyNode;6;-370.4583,180.8134;Float;True;Property;_TriplanarNormal1;Triplanar Normal;3;0;Create;True;0;0;False;0;None;None;True;bump;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.TexturePropertyNode;7;-370.4583,-27.18655;Float;True;Property;_TopNormal1;Top Normal;1;0;Create;True;0;0;False;0;None;f1bc415e28d17ad42a1bb8624a8a193a;False;bump;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.TriplanarNode;8;45.54175,-408.6013;Inherit;True;Cylindrical;World;False;Top Texture 0;_TopTexture0;white;-1;None;Mid Texture 0;_MidTexture0;white;-1;None;Bot Texture 0;_BotTexture0;white;-1;None;Triplanar Sampler;False;10;0;SAMPLER2D;;False;5;FLOAT;1;False;1;SAMPLER2D;;False;6;FLOAT;0;False;2;SAMPLER2D;;False;7;FLOAT;0;False;9;FLOAT3;0,0,0;False;8;FLOAT3;1,1,1;False;3;FLOAT2;1,1;False;4;FLOAT;2;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;14;258.99,-531.3674;Inherit;False;Property;_alpha;alpha;9;0;Create;True;0;0;False;0;0.8215393;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;13;306.9863,-751.504;Inherit;False;Property;_albeto;albeto;8;0;Create;True;0;0;False;0;0.1123238,0.04223033,0.1226415,0;0.5377358,0.3424261,0.5333728,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;9;141.5417,148.8134;Float;False;Property;_Smoothness1;Smoothness;5;0;Create;True;0;0;False;0;0.5;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;10;141.5417,68.81345;Float;False;Property;_Specular1;Specular;4;0;Create;True;0;0;False;0;0.02;0.02;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TriplanarNode;11;45.54175,-155.1866;Inherit;True;Cylindrical;World;True;Top Texture 1;_TopTexture1;white;-1;None;Mid Texture 1;_MidTexture1;white;-1;None;Bot Texture 1;_BotTexture1;white;-1;None;Triplanar Sampler;False;10;0;SAMPLER2D;;False;5;FLOAT;1;False;1;SAMPLER2D;;False;6;FLOAT;0;False;2;SAMPLER2D;;False;7;FLOAT;0;False;9;FLOAT3;0,0,0;False;8;FLOAT3;1,1,1;False;3;FLOAT2;1,1;False;4;FLOAT;2;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BlendOpsNode;12;617.8771,-515.908;Inherit;True;HardLight;True;3;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;926.8286,-331.5713;Float;False;True;2;ASEMaterialInspector;0;0;Standard;ColoredTriplanar;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;8;0;2;0
WireConnection;8;1;3;0
WireConnection;8;2;3;0
WireConnection;8;9;4;0
WireConnection;8;3;1;0
WireConnection;8;4;5;0
WireConnection;11;0;7;0
WireConnection;11;1;6;0
WireConnection;11;2;6;0
WireConnection;11;9;4;0
WireConnection;11;3;1;0
WireConnection;11;4;5;0
WireConnection;12;0;13;0
WireConnection;12;1;8;0
WireConnection;12;2;14;0
WireConnection;0;0;12;0
WireConnection;0;1;11;0
WireConnection;0;3;10;0
WireConnection;0;4;9;0
ASEEND*/
//CHKSM=26BDA401F240A4CC4E7521853449605F6D5C41B5