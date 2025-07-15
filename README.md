# 🎨 TexMergeApp

**TexMergeApp** is a desktop tool for combining texture map sets into optimized PBR texture outputs. It is designed to streamline the workflow between **Substance Painter** and real-time rendering engines by merging and packing exported texture layers into final optimized maps.

---

## 🚀 Features

✅ Merge texture sets from multiple material layers into unified output maps  
✅ Pack AO, Roughness, and Metallic into RGB channels of a single texture  
✅ Supports Base Color, Normal, AO, Roughness, Metallic, and other PBR maps  
✅ Minimalist and fast UI built with **Blazor** and **MudBlazor**

---

## 🛠 Usage

1. Create your 3D model in **Blender** and assign **Material IDs**.
2. Import the model into **Substance Painter** and create your texture layers.
3. When exporting textures from Substance Painter:
   - Use `Dilation + transparent`
   - Add padding (e.g., **2px**)
   - Export in **`.png`** format
   - Ensure the **Base Color export includes an alpha channel** (used as mask)
4. Launch **TexMergeApp**.
5. Select the **Input Folder** (where textures from Substance Painter were exported).
6. Select the **Output Folder**.
7. Choose your desired output options (e.g., Merge, Pack Extra).
8. Click **Merge Textures**.

---

## 🧩 Packaging Explanation

**TexMergeApp** is designed to combine texture map sets exported from **Substance Painter**, where each exported texture corresponds to a specific **layer set**—such as **Base Color**, **Roughness**, **Ambient Occlusion (AO)**, **Metallic**, **Normals**, etc.

### 🔄 What TexMergeApp Does:

#### 1. Merging Same-Type Textures Across Layers
All textures of the same type (e.g., all **AO** maps, all **Roughness** maps) from different layers are merged into a **single corresponding output texture**.

#### 2. Packing Multiple Maps into a Single Texture
Optionally, TexMergeApp allows you to **pack multiple maps into a single texture** using separate **color channels**:

- 🔴 **Red channel** → Ambient Occlusion (AO)  
- 🟢 **Green channel** → Roughness  
- 🔵 **Blue channel** → Metallic

This is a common **optimization technique** to reduce disk space and file count. ⚠️ However, this approach **requires custom logic** in the target application (e.g., custom shaders or material nodes in your game engine) to **unpack** the individual maps from each channel.

### 📦 Output
The result is a compact set of **packed textures**, with **one optimized texture per map type**, suitable for **efficient real-time rendering workflows**.

### 📁 Input Format
- ✅ Input files must be in **`.png`** format
- ✅ Ensure that the **alpha channel is preserved** in Base Color maps (used as mask during merging)

---

## 📂 License
This tool is open source and available under the **MIT License**.

---

## 🔗 Repository
To contribute or file an issue, visit the [GitHub repository](https://github.com/Dimetroc/TexMergeApp).

