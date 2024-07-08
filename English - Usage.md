Firstly, this resource is extended to Unity's new version of Navmesh technology, which submits necessary parameters for baking grid data at the code level to achieve real-time baking and utilization. In addition, this resource can be extended to more interfaces. For example, if developers design certain building systems, when new objects are placed within the Bounds boundary range, they need to re bake the grid, and of course, Unity's official dynamic occlusion technology can also be used appropriately. This Cs script is suitable for large game scenes and complex scene level designs. As it does not generate grids of all scene sizes at once, it is also a relatively violent optimization method. It only uses some Cpu resources for baking in a short period of time, and the calculated functions are roughly encapsulated in an IEnumerator program. That is to say, there is still a lot of optimization space in this CS script. Currently, what we are sharing with you is only the implementation of a function, teaching you how to use custom data to build grids. Finally, since the generated grids are always stored in memory and there is only one at most, developers may need to focus on Fine tune the boundary range based on the complexity of your own project scenario, To avoid frequent CPU consumption or excessive memory usage. However, in the end, it is better than generating all the meshes at once, and there is a significant improvement in performance.

usage:

1. In the scene where this script is needed, you need to create a new empty object (which can be in any position, but cannot be a child of a player or a moving object). Assuming you name this new empty object "NavMeshBuilder", then you need to add the NavMeshSurface component and NavMeshBuild component to the "NavMeshBuilder" object.

2. Fill in their values based on the parameter descriptions in the image![动态网格英文版](https://github.com/normandata/Unity3D-Dynamic-Navmesh-Build-Navmesh-/assets/125154193/730e3243-e564-4263-80c3-dd5ba77075a9)
3. Ensure that the Target Layer in the NavMeshBuild component is not empty.
4. Run tests
Provided interface:
Inheritance class: NavMeshBuild.NavMeshs
Provided function: 1. void OnNavMeshDataChange();
Function: When grid data changes, trigger the function.
Usage example:
![leisi](https://github.com/normandata/Unity3D-Dynamic-Navmesh-Build-Navmesh-/assets/125154193/3b6dcda4-35e5-4db2-9b14-0d4caa5dcbdf)
