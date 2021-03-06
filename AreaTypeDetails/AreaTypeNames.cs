using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lutron.Gulliver.DomainObjects.AreaTypeNamespace
{
	/// <summary>
	/// Enumerates all the AreaTypes that can be predicted by the FuzzyMatch and ML model
	/// Based on enum values for Caseta AreaCategories and expanded for Lutron Designer
	/// </summary>
	public enum AreaTypeNames
	{
		/// <summary>
		/// Enum for Unset AreaType
		/// </summary>
		Unset = 0,

		/// <summary>
		/// Enum for Kitchen AreaType
		/// </summary>
		Kitchen = 1,

		/// <summary>
		/// Enum for LivingRoom AreaType
		/// </summary>
		LivingRoom = 2,

		/// <summary>
		/// Enum for DiningRoom AreaType
		/// </summary>
		DiningRoom = 3,

		/// <summary>
		/// Enum for MasterBedroom AreaType
		/// </summary>
		MasterBedroom = 4,

		/// <summary>
		/// Enum for FrontPorch AreaType
		/// </summary>
		FrontPorch = 5,

		/// <summary>
		/// Enum for FrontFoyer AreaType
		/// </summary>
		FrontFoyer = 6,

		/// <summary>
		/// Enum for GarageEntry AreaType
		/// </summary>
		GarageEntry = 7,

		/// <summary>
		/// Enum for BackEntry AreaType
		/// </summary>
		BackEntry = 8,

		/// <summary>
		/// Enum for Mudroom AreaType
		/// </summary>
		Mudroom = 9,

		/// <summary>
		/// Enum for Exterior AreaType
		/// </summary>
		Exterior = 10,

		/// <summary>
		/// Enum for FamilyRoom AreaType
		/// </summary>
		FamilyRoom = 11,

		/// <summary>
		/// Enum for Garage AreaType
		/// </summary>
		Garage = 12,

		/// <summary>
		/// Enum for Office AreaType
		/// </summary>
		Office = 13,

		/// <summary>
		/// Enum for Bedroom AreaType
		/// </summary>
		Bedroom = 14,

		/// <summary>
		/// Enum for Bathroom AreaType
		/// </summary>
		Bathroom = 15,

		/// <summary>
		/// Enum for BasementSitting AreaType
		/// </summary>
		BasementSitting = 16,

		/// <summary>
		/// Enum for BasementGame AreaType
		/// </summary>
		BasementGame = 17,

		/// <summary>
		/// Enum for BasementExercise AreaType
		/// </summary>
		BasementExercise = 18,

		/// <summary>
		/// Enum for BasementWork AreaType
		/// </summary>
		BasementWork = 19,

		/// <summary>
		/// Enum for BasementStorage AreaType
		/// </summary>
		BasementStorage = 20,

		/// <summary>
		/// Enum for BasementBathroom AreaType
		/// </summary>
		BasementBathroom = 21,

		/// <summary>
		/// Enum for BasementStairs AreaType
		/// </summary>
		BasementStairs = 22,

		/// <summary>
		/// Enum for DownstairsHallway AreaType
		/// </summary>
		DownstairsHallway = 23,

		/// <summary>
		/// Enum for UpstairsHallway AreaType
		/// </summary>
		UpstairsHallway = 24,

		/// <summary>
		/// Enum for Stairs AreaType
		/// </summary>
		Stairs = 25,

		/// <summary>
		/// Enum for Closet AreaType
		/// </summary>
		Closet = 26,

		/// <summary>
		/// Enum for LaundryRoom AreaType
		/// </summary>
		LaundryRoom = 27,

		/// <summary>
		/// Enum for Playroom AreaType
		/// </summary>
		Playroom = 28,

		/// <summary>
		/// Enum for Den AreaType
		/// </summary>
		Den = 29,

		/// <summary>
		/// Enum for Gym AreaType
		/// </summary>
		Gym = 30,

		/// <summary>
		/// Enum for RecRoom AreaType
		/// </summary>
		RecRoom = 31,

		/// <summary>
		/// Enum for Theater AreaType
		/// </summary>
		Theater = 32,

		/// <summary>
		/// Enum for MediaRoom AreaType
		/// </summary>
		MediaRoom = 33,

		/// <summary>
		/// Enum for GreatRoom AreaType
		/// </summary>
		GreatRoom = 34,

		/// <summary>
		/// Enum for Study AreaType
		/// </summary>
		Study = 35,

		/// <summary>
		/// Enum for Library AreaType
		/// </summary>
		Library = 36,

		/// <summary>
		/// Enum for Bar AreaType
		/// </summary>
		Bar = 37,

		/// <summary>
		/// Enum for Sunroom AreaType
		/// </summary>
		Sunroom = 38,

		/// <summary>
		/// Enum for Nook AreaType
		/// </summary>
		Nook = 39,

		/// <summary>
		/// Enum for Pantry AreaType
		/// </summary>
		Pantry = 40,

		/// <summary>
		/// Enum for UtilityRoom AreaType
		/// </summary>
		UtilityRoom = 41,

		/// <summary>
		/// Enum for CustomCooking AreaType
		/// </summary>
		CustomCooking = 42,

		/// <summary>
		/// Enum for CustomDining AreaType
		/// </summary>
		CustomDining = 43,

		/// <summary>
		/// Enum for CustomRecreation AreaType
		/// </summary>
		CustomRecreation = 44,

		/// <summary>
		/// Enum for CustomWorking AreaType
		/// </summary>
		CustomWorking = 45,

		/// <summary>
		/// Enum for CustomEntertaining AreaType
		/// </summary>
		CustomEntertaining = 46,

		/// <summary>
		/// Enum for CustomSleeping AreaType
		/// </summary>
		CustomSleeping = 47,

		/// <summary>
		/// Enum for CustomExercising AreaType
		/// </summary>
		CustomExercising = 48,

		/// <summary>
		/// Enum for CustomEntry AreaType
		/// </summary>
		CustomEntry = 49,

		/// <summary>
		/// Enum for CustomTransition AreaType
		/// </summary>
		CustomTransition = 50,

		/// <summary>
		/// Enum for CustomStorage AreaType
		/// </summary>
		CustomStorage = 51,

		/// <summary>
		/// Enum for CustomOutdoors AreaType
		/// </summary>
		CustomOutdoors = 52,

		/// <summary>
		/// Enum for Hallway AreaType
		/// </summary>
		Hallway = 53,

		/// <summary>
		/// Enum for CustomBathroom AreaType
		/// </summary>
		CustomBathroom = 54,

		/// <summary>
		/// Enum for Atrium AreaType
		/// </summary>
		Atrium = 55,

		/// <summary>
		/// Enum for Breakroom AreaType
		/// </summary>
		Breakroom = 56,

		/// <summary>
		/// Enum for Classroom AreaType
		/// </summary>
		Classroom = 57,

		/// <summary>
		/// Enum for ConferenceRoom AreaType
		/// </summary>
		ConferenceRoom = 58,

		/// <summary>
		/// Enum for OpenOffice AreaType
		/// </summary>
		OpenOffice = 59,

		/// <summary>
		/// Enum for PrivateOffice AreaType
		/// </summary>
		PrivateOffice = 60,

		/// <summary>
		/// Enum for Ballroom AreaType
		/// </summary>
		Ballroom = 61,

		/// <summary>
		/// Enum for Reception AreaType
		/// </summary>
		Reception = 62,

		/// <summary>
		/// Enum for Dormitory AreaType
		/// </summary>
		Dormitory = 63,

		/// <summary>
		/// Enum for Cafeteria AreaType
		/// </summary>
		Cafeteria = 64,

		/// <summary>
		/// Enum for PatientRoom AreaType
		/// </summary>
		PatientRoom = 65,

		/// <summary>
		/// Enum for Lab AreaType
		/// </summary>
		Lab = 66,

		/// <summary>
		/// Enum for GuestRoom AreaType
		/// </summary>
		GuestRoom = 67,

		/// <summary>
		/// Enum for Other AreaType
		/// </summary>
		Other = 68,

		/// <summary>
		/// Enum for CollaborationRoom AreaType
		/// </summary>
		CollaborationRoom = 69,

		/// <summary>
		/// Enum for ConsultationRoom AreaType
		/// </summary>
		ConsultationRoom = 70,

		/// <summary>
		/// Enum for CopyRoom AreaType
		/// </summary>
		CopyRoom = 71,

		/// <summary>
		/// Enum for ElectricalMechanicalRoom AreaType
		/// </summary>
		ElectricalMechanicalRoom = 72,

		/// <summary>
		/// Enum for ExamRoom AreaType
		/// </summary>
		ExamRoom = 73,

		/// <summary>
		/// Enum for JanitorialCloset AreaType
		/// </summary>
		JanitorialCloset = 74,

		/// <summary>
		/// Enum for LibraryReadingRoom AreaType
		/// </summary>
		LibraryReadingRoom = 75,

		/// <summary>
		/// Enum for LibraryStack AreaType
		/// </summary>
		LibraryStack = 76,

		/// <summary>
		/// Enum for Lobby AreaType
		/// </summary>
		Lobby = 77,

		/// <summary>
		/// Enum for LockerRoom AreaType
		/// </summary>
		LockerRoom = 78,

		/// <summary>
		/// Enum for Lounge AreaType
		/// </summary>
		Lounge = 79,

		/// <summary>
		/// Enum for MultipurposeRoom AreaType
		/// </summary>
		MultipurposeRoom = 80,

		/// <summary>
		/// Enum for ParkingGarage AreaType
		/// </summary>
		ParkingGarage = 81,

		/// <summary>
		/// Enum for StorageRoom AreaType
		/// </summary>
		StorageRoom = 82,

		/// <summary>
		/// Enum for Warehouse AreaType
		/// </summary>
		Warehouse = 83,

		/// <summary>
		/// Enum for WellnessRoom AreaType
		/// </summary>
		WellnessRoom = 84,

		/// <summary>
		/// Enum for Foyer/Entry AreaType
		/// </summary>
		Foyer_Entry = 85,

		/// <summary>
		/// Enum for Hallway/Stairs AreaType
		/// </summary>
		Hallway_Stairs = 86,

		/// <summary>
		/// Enum for WineCellar AreaType
		/// </summary>
		WineCellar = 87,

		/// <summary>
		/// Enum for EntertainingRoom AreaType
		/// </summary>
		EntertainingRoom = 88,

		/// <summary>
		/// Enum for Porch AreaType
		/// </summary>
		Porch = 89,

		/// <summary>
		/// Enum for PowderRoom AreaType
		/// </summary>
		PowderRoom = 90,

		/// <summary>
		/// Enum for Clothing Closet AreaType
		/// </summary>
		ClothingCloset = 91,

		/// <summary>
		/// Enum for Closet/Storage AreaType
		/// </summary>
		Closet_Storage = 92,

		/// <summary>
		/// Enum for Pool AreaType
		/// </summary>
		Pool = 93,

		/// <summary>
		/// Enum for OutdoorsLiving AreaType
		/// </summary>
		OutdoorsLiving = 94,

		/// <summary>
		/// Enum for Art Room/Gallery AreaType
		/// </summary>
		ArtRoom_Gallery = 95,

		/// <summary>
		/// Enum for CraftRoom AreaType
		/// </summary>
		CraftRoom = 96,

		/// <summary>
		/// Enum for Others AreaType
		/// </summary>
		Others = 97

	}
}