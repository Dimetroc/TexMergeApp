using ImageMagick;

namespace TexMerge
{
    internal static class Constants
    {
        public static string[] Color = [ "_Base_Color", "_BaseColor", "_BC" ];
        public static MagickColor ColorColor = new ("#000000");
        //Base set
        public static string[] Roughness = [ "_Roughness", "_Rough", "_R", "-Roughness", "-Rough", "-R" ];
        public static MagickColor RoughnessColor = new ("#ffffff");
        public static string[] Metallic = [ "_Metallic", "_Metal", "_M", "-Metallic", "-Metal", "-M" ];
        public static MagickColor MetalliColor = new("#000000");
        public static string[] Ao = [ "_Mixed_AO", "_AmbientOcclusion", "_Ambient_Occlusion", "_AO", "-Mixed_AO", "-AmbientOcclusion", "-Ambient-Occlusion", "-AO" ];
        public static MagickColor AoColor = new ("#ffffff");
        public static string[] NormalDX = [ "_Normal_DirectX", "_NormalDirectX", "_NormalDX", "_NDX" ];
        public static MagickColor NormalDXColor = new ("#7f80ff");
        public static string[] NormalOGL = [ "_Normal_OpenGl", "_NormalOpenGL", "_NormalOGL", "_NOGL"];
        public static MagickColor NormalOGLColor = new ("#7f80ff");
        public static string[] Normal = [ "_Normal", "_N" ];
        public static MagickColor NormalColor = new ("#7f80ff");
        //Extra set
        public static string[] Height = [ "_Height", "_High", "_H" ];
        public static MagickColor HeightColor = new ("#848484");
        public static string[] Emissive = [ "_Emissive", "_E" ];
        public static MagickColor EmissiveColor = new ("#848484");
        public static string[] Diffuse = [ "_Diffuse", "_Diff", "_Dif", "_DF", "_D" ];
        public static MagickColor DiffusColor = new("#848484");
        public static string[] Specular = [ "_Specular", "_Spec", "_S" ];
        public static MagickColor SpecularColor = new ("#848484");
        public static string[] Glossiness = [ "_Glossiness", "_Glos", "_G" ];
        public static MagickColor GlossinesColor = new("#000000");
        public static string[] Displacement = [ "_Displacement", "_Disp", "_DP" ];
        public static MagickColor DisplacementColor = new("#000000");
        public static string[] Refraction = [ "_IndexOfRefraction", "_Refraction", "_IOR" ];
        public static MagickColor RefractionColor = new("#000000");
        public static string[] Reflection = [ "_Reflection", "_RF" ];
        public static MagickColor ReflectionColor = new("#000000");
        public const string AoRM = "_AO_R_M";
    }
}