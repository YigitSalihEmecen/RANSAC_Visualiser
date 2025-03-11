# RANSAC_Visualiser
Lidar Simulation & RANSAC Visualization
This Unity project simulates Lidar data and visualizes the RANSAC (Random Sample Consensus) algorithm to detect the best-fitting line in a noisy dataset.

Features
✔️ Lidar Data Simulation – Generates random points along a line with configurable noise and outliers.
✔️ RANSAC Algorithm Implementation – Iteratively selects lines to maximize inliers and visualize the best-fitting line.
✔️ Interactive UI Controls – Adjustable parameters for noise, outlier count, slope, and iteration speed.
✔️ Data Table Visualization – Displays results in a structured table.
✔️ Perpendicular Distance Calculation – Visualizes inliers and outliers using perpendicular lines.

Installation & Usage
1. Setup the Project
Open Unity (ensure it's a recent version).
Import this project into Unity.
2. Scene Setup
Load SampleScene in the Unity Editor.
Ensure the necessary UI elements, prefabs, and GameObjects (such as pointPrefab, rowPrefab, perpendicularLinePrefab) are assigned in the Inspector.
3. Running the Simulation
Adjust parameters such as numPoints, noiseLevel, and outlierCount using the UI sliders and input fields.
Click "Generate" to create a new set of points.
Click "Run RANSAC" to start the iterative process.
The best-fitting line is visualized after iterations are completed.
Code Structure
Main Components
Controller.cs 🏗️

Generates simulated Lidar data.
Implements RANSAC for line fitting.
Handles UI interactions and updates visuals.
MeasurementData.cs 📊

Defines the structure to store point pairs, inliers, and outliers.
MeasurementTable.cs 📋

Displays inlier and outlier counts for each tested line.
Adjustable Parameters
Parameter	Description
numPoints	Total number of points in the simulation.
noiseLevel	Amount of noise added to inliers.
outlierCount	Number of outliers randomly placed.
maxRange	Defines the Lidar scanning range.
slope	The slope of the original line.
p (Threshold)	Perpendicular distance for inlier detection.
iterations	Number of RANSAC iterations.
simulationSpeed	Controls the speed of the RANSAC process.
Future Improvements
🚀 Implement additional RANSAC models (e.g., circle fitting).
📈 Improve UI for a more intuitive experience.
🖥️ Export inlier/outlier data for analysis.

License
This project is open-source and free to use for educational purposes.
