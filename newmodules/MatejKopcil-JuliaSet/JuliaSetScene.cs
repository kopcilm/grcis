using MatejKopcil;

//////////////////////////////////////////////////
// Rendering params.

Debug.Assert(scene != null);
Debug.Assert(context != null);

//////////////////////////////////////////////////
// CSG scene:
CSGInnerNode root = new CSGInnerNode(SetOperation.Union);
root.SetAttribute(PropertyName.REFLECTANCE_MODEL, new PhongModel());
root.SetAttribute(PropertyName.MATERIAL, new PhongMaterial(new double[] {1.0, 0.6, 0.1}, 0.1, 0.6, 0.4, 16));
scene.Intersectable = root;

// Background color:
scene.BackgroundColor = new double[] {0.0, 0.0, 0.0};

// Camera:
scene.Camera = new StaticCamera(new Vector3d(3, 0.1, -3),
                             new Vector3d(-1.0, 0.0, 1.0),
                             50.0);

// Light sources:
scene.Sources = new LinkedList<ILightSource>();
scene.Sources.Add(new AmbientLightSource(0.1));
scene.Sources.Add(new PointLightSource(new Vector3d(0,2,0), 1.0));

// --- NODE DEFINITIONS ----------------------------------------------------

JuliaSet js =
new JuliaSet(-0.8, 0.2, 0, 0) { Epsilon = 0.001, MaxIterations = 20 };

js.SetAttribute(PropertyName.COLOR, new double[]{0,0,0});
js.SetAttribute(PropertyName.TEXTURE, new NormalTexture());

root.InsertChild(js, Matrix4d.Identity);
