using MatejKopcil;

//////////////////////////////////////////////////
// Rendering params.

Debug.Assert(scene != null);
Debug.Assert(context != null);

//////////////////////////////////////////////////
// CSG scene:
CSGInnerNode root = new CSGInnerNode( SetOperation.Union );
root.SetAttribute( PropertyName.REFLECTANCE_MODEL, new PhongModel() );
root.SetAttribute( PropertyName.MATERIAL, new PhongMaterial( new double[] { 1.0, 0.8, 0.1 }, 0.1, 0.6, 0.4, 128 ) );
scene.Intersectable = root;

root.SetAttribute(PropertyName.CTX_SUPERSAMPLING, new SupersamplingImageSynthesizer());

// Background color:
scene.BackgroundColor = new double[] { 0.0, 0.05, 0.07 };

// Camera:
scene.Camera = new DepthOfFieldCamera(new Vector3d(0.25,0.5,-2.4),
                                   2,
                                   new Vector3d( -1.0, 0.75, -4.2 ),
                                   new Vector3d( 0.55, -0.25, 1.0 ),
                                   50.0 );

// Light sources:
scene.Sources = new LinkedList<ILightSource>();
scene.Sources.Add( new AmbientLightSource( 0.8 ) );
scene.Sources.Add( new PointLightSource( new Vector3d( -5.0, 4.0, -3.0 ), 1.2 ) );

// --- NODE DEFINITIONS ----------------------------------------------------

// Params dictionary:
Dictionary<string, string> p = Util.ParseKeyValueList( param );

// n = <index-of-refraction>
double n = 1.6;
Util.TryParse( p, "n", ref n );


Cube c1 = new Cube();
Cube c2 = new Cube();
Cube c3 = new Cube();
Cube c4 = new Cube();
Cube c5 = new Cube();
c1.SetAttribute(PropertyName.COLOR, new double[]{1,0,0});
c2.SetAttribute(PropertyName.COLOR, new double[]{1,1,0});
c3.SetAttribute(PropertyName.COLOR, new double[]{0,1,1});
c4.SetAttribute(PropertyName.COLOR, new double[]{0,0,1});
c5.SetAttribute(PropertyName.COLOR, new double[]{1,1,1});
root.InsertChild(c1, Matrix4d.Identity * Matrix4d.RotateY(45) * Matrix4d.Scale(0.5) * Matrix4d.CreateTranslation(0.0, 0, -2.4));
root.InsertChild(c2, Matrix4d.Identity * Matrix4d.Scale(0.625) * Matrix4d.CreateTranslation(0.15, 0, -2));
root.InsertChild(c3, Matrix4d.Identity * Matrix4d.Scale(0.75) * Matrix4d.CreateTranslation(0.05, 0, -1));
root.InsertChild(c4, Matrix4d.Identity * Matrix4d.Scale(1) * Matrix4d.CreateTranslation(0.0, 0, 0));
root.InsertChild(c5, Matrix4d.Identity * Matrix4d.Scale(0.25) * Matrix4d.CreateTranslation(-0.2, 0, -3.2));

// Infinite plane with checker:
Plane pl = new Plane();
pl.SetAttribute( PropertyName.COLOR, new double[] { 0.2, 0.2, 0.2 } );
pl.SetAttribute( PropertyName.TEXTURE, new CheckerTexture( 2, 2, new double[] { 0.1,0.1,0.1 } ) );
root.InsertChild( pl, Matrix4d.RotateX( -MathHelper.PiOver2 ) * Matrix4d.CreateTranslation( 0.0, 0.0, 0.0 ) );
