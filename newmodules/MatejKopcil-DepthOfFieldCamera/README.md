# Extension: DepthOfFieldCamera

![Example](DOF.png)

### Author: Matej Kopcil

### Category: Camera

### Namespace: MatejKopcil

### Class name: DepthOfFieldCamera : StaticCamera, ICamera

### ITimeDependent: No

### Source file: DepthOfFieldCamera.cs

Simulates depth of field camera effect. Dof is simulated by multisampling. User define DoF camera by 2 parameters: 

* **Focal length** - determines how far objects must be from the camera to be in focus. Given eater by exact value or by a point which should be in focus.  
* **Aperture angle** -  determine how blurry objects that are out of focus will appear.

and by parameters of `StaticCamera`.
## Example

From a scene definition script
```
using MatejKopcil;

...

scene.Camera = new DepthOfFieldCamera(new Vector3d(0.25, 0.5, -2.4),
                                      2,
                                      new Vector3d( -1.0, 0.75, -4.2 ),
                                      new Vector3d( 0.55, -0.25, 1.0 ),
                                      50.0 );
```


### Sample scene script: DepthOfFieldCameraScene.cs
