using CommunityToolkit.Maui.Alerts;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using FoodNinja.Model;
using FoodNinja.Pages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodNinja.Services
{
    public class FirebaseManager
    {
        private const string FirebaseApiKey = "AIzaSyD0ehwCKtxucZLyNcUGv-ZFKaNXmw_cmK8";
        private const string DatabaseUrl = "https://fir-maui-491c3-default-rtdb.firebaseio.com/";
        FirebaseAuthProvider firebaseAuthProvider;
        FirebaseClient firebaseClient;

        public FirebaseManager()
        {
            firebaseAuthProvider = new FirebaseAuthProvider(new FirebaseConfig(FirebaseApiKey));
            firebaseClient = new FirebaseClient(DatabaseUrl);
        }

        public async Task<SignupModel> CreateAccount(string userName, string email, string password)
        {
            try
            {

                var response = await firebaseAuthProvider.CreateUserWithEmailAndPasswordAsync(email, password, userName);
                var veri = firebaseAuthProvider.SendEmailVerificationAsync(response);
                if (response != null)
                {
                    Preferences.Set("UserId", response.User.LocalId);
                    return new SignupModel
                    {
                        Id = response.User.LocalId,
                        UserName = userName,
                        Email = response.User.Email,
                        Password = password
                    };
                }
                return null;
            }
            catch (FirebaseAuthException ex)
            {
                Console.WriteLine("Error while creating new account" + ex.Message);
                if (ex.Reason == AuthErrorReason.WeakPassword)
                {
                    await Toast.Make(ex.Message).Show();
                }
                if (ex.Reason == AuthErrorReason.EmailExists)
                {
                    await Toast.Make(ex.Message).Show();
                }
                return null;
            }
        }
        public async Task<SignupModel> LoginAsync(string email, string password)
        {
            try
            {
                var response = await firebaseAuthProvider.SignInWithEmailAndPasswordAsync(email, password);
                if (response != null)
                {
                    if (response.User.IsEmailVerified)
                    {
                        Preferences.Set("IsLoggedIn", true);
                        Preferences.Set("FirebaseToken", response.FirebaseToken);
                        Preferences.Set("RefereshToken", response.RefreshToken);
                        Preferences.Set("Name", response.User.DisplayName);
                        Preferences.Set("LocalId", response.User.LocalId);
                        return new SignupModel
                        {
                            Id = response.User.LocalId,
                            UserName = response.User.DisplayName,
                            Email = response.User.Email,
                            Token = response.FirebaseToken,
                            RefreshToken = response.RefreshToken,
                        };
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Error", "Please verify your email for login", "Ok");
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (FirebaseAuthException ex)
            {
                Console.WriteLine("Error while creating new account" + ex.Message);
                if (ex.Message.Contains("INVALID_LOGIN_CREDENTIALS"))
                {
                    await Toast.Make("Invalid login credentials").Show();
                }
                else
                {
                    switch (ex.Reason)
                    {
                        case AuthErrorReason.UnknownEmailAddress:
                            await Toast.Make("Unknown email address").Show();
                            break;
                        case AuthErrorReason.InvalidIDToken:
                            await Toast.Make("Invalid ID token").Show();
                            break;
                        case AuthErrorReason.InvalidEmailAddress:
                            await Toast.Make("Invalid email address").Show();
                            break;
                        case AuthErrorReason.InvalidAccessToken:
                            await Toast.Make("Invalid access token").Show();
                            break;
                        case AuthErrorReason.SystemError:
                            await Toast.Make("System error occurred").Show();
                            break;
                        case AuthErrorReason.WrongPassword:
                            await Toast.Make("Wrong password").Show();
                            break;
                        case AuthErrorReason.UserDisabled:
                            await Toast.Make("User account is disabled").Show();
                            break;
                        case AuthErrorReason.UserNotFound:
                            await Toast.Make("User not found").Show();
                            break;
                        case AuthErrorReason.TooManyAttemptsTryLater:
                            await Toast.Make("Too many attempts, try again later").Show();
                            break;
                        default:
                            await Toast.Make("An unknown error occurred! Try Again Later.").Show();
                            break;
                    }
                }
                return null;
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine("General error during login: " + ex.Message); // Debug log
                throw;
            }
        }
        public async Task<string> RefereshFirebaseTokenAsync(FirebaseAuth auth)
        {
            try
            {
                var refreshedAuth = await firebaseAuthProvider.RefreshAuthAsync(auth);
                string newToken = refreshedAuth.FirebaseToken;
                Preferences.Set("FirebaseToken", newToken);
                Preferences.Set("RefreshToken", refreshedAuth.RefreshToken);
                return newToken;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Token refresh failed: " + ex.Message);
                return null;
            }
        }
        public async Task<bool> UploadUserDataAsync(UserDataModel userData)
        {
            try
            {
                userData.Id = Preferences.Get("UserId", string.Empty);
                var firebaseClient = new FirebaseClient(DatabaseUrl);
                var response = await firebaseClient
                    .Child("UserData")
                    .Child(userData.Id)
                    .PostAsync(JsonConvert.SerializeObject(userData));
                return true;
            }
            catch (Exception ex)
            {
                await Toast.Make(ex.Message).Show();
                await App.Current.MainPage.DisplayAlert("Error", ex.Message, "Ok");
                Console.WriteLine("Error while uploading user data" + ex.Message);
                return false;
            }
        }
        public async Task<List<NearestRestaurantModel>> GetNearestRestaurantAsync()
        {
            try
            {
                var restaurantData = await firebaseClient
                .Child("NearestRestro")
                .OnceAsync<NearestRestaurantModel>();
                var restaurants = restaurantData
                    .Select(item => new NearestRestaurantModel
                    {
                        Id = item.Object.Id,
                        RestaurantName = item.Object.RestaurantName,
                        RestrauntDistance = item.Object.RestrauntDistance,
                        RestaurantImage = item.Object.RestaurantImage,
                        RestaurantRating = item.Object.RestaurantRating,
                        FoodItems = item.Object.FoodItems,
                        Description = item.Object.Description,
                        RestroAddress = item.Object.RestroAddress,
                        RestaurantLat = item.Object.RestaurantLat,
                        RestaurantLong = item.Object.RestaurantLong
                    }).ToList();
                return restaurants;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while fetching nearest restaurant data" + ex.Message);
                return new List<NearestRestaurantModel>();
            }
        }
        public async Task<List<PopularMenuModel>> GetPopularMenuListAync()
        {
            try
            {
                var popularMenuData = await firebaseClient
                    .Child("PopularMenu")
                    .OnceAsync<PopularMenuModel>();
                var popularMenus = popularMenuData
                    .Select(item => new PopularMenuModel
                    {
                        Id = item.Object.Id,
                        RestaurantName = item.Object.RestaurantName,
                        DishName = item.Object.DishName,
                        FoodImage = item.Object.FoodImage,
                        Price = item.Object.Price,
                        RestaurantId = item.Object.RestaurantId,
                        FoodRating = item.Object.FoodRating,
                        RestaurantDescription = item.Object.RestaurantDescription,
                        FoodDescription = item.Object.FoodDescription,
                        RestaurantAddress = item.Object.RestaurantAddress,
                        RestaurantLat = item.Object.RestaurantLat,
                        RestaurantLong = item.Object.RestaurantLong,
                    }).ToList();
                return popularMenus;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error While fetching popular menu data" + ex.Message);
                return new List<PopularMenuModel>();
            }
        }
        public async Task<bool> AddFoodToCartAsync(AddFoodToCart addFoodToCart)
        {
            try
            {
                addFoodToCart.UserId = Preferences.Get("LocalId", string.Empty);
                var firebaseClient = new FirebaseClient(DatabaseUrl);

                // Check if the item already exists in the cart (matching UserId, RestaurantId, and FoodId)
                var existingItems = await firebaseClient
                   .Child("CartItem")
                   .Child(addFoodToCart.UserId)
                   .Child(addFoodToCart.RestaurantId.ToString())
                   .OnceAsync<AddFoodToCart>();
                var existingItem = existingItems
                    .FirstOrDefault(item => item.Object.FoodId == addFoodToCart.FoodId);
                if (existingItem != null)
                {
                    // Item exists, update the quantity
                    var updatedFood = existingItem.Object;
                    updatedFood.Quantity += 1;
                    await firebaseClient
                        .Child("CartItem")
                        .Child(addFoodToCart.UserId)
                        .Child(addFoodToCart.RestaurantId.ToString())
                        .Child(existingItem.Key)
                        .PutAsync(updatedFood);
                    return true;
                }
                else
                {
                    // Item does not exist, add new
                    await firebaseClient
                        .Child("CartItem")
                        .Child(addFoodToCart.UserId)
                        .Child(addFoodToCart.RestaurantId.ToString())
                        .PostAsync(addFoodToCart);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while uploading detail for cart: {ex.Message}\nStack Trace: {ex.StackTrace}");
                await Toast.Make(ex.Message).Show();
                return false;
            }
        }
        public async Task<List<AddFoodToCart>> GetCartDataAsync()
        {
            try
            {
                var userId = Preferences.Get("LocalId", string.Empty);
                var restaurantNodes = await firebaseClient
                   .Child("CartItem")
                   .Child(userId)
                   .OnceAsync<object>();
                if (restaurantNodes == null) return new List<AddFoodToCart>();
                var cartItemsList = new List<AddFoodToCart>();
                foreach (var restaurantNode in restaurantNodes)
                {
                    // Fetch all food items for the current restaurant in parallel
                    var foodItems = await firebaseClient
                        .Child("CartItem")
                        .Child(userId)
                        .Child(restaurantNode.Key) // Access the specific restaurant node
                        .OnceAsync<AddFoodToCart>(); // Fetch all food items under this restaurant

                    // Add all food items to the cart list using LINQ
                    cartItemsList.AddRange(foodItems.Select(foodItem => new AddFoodToCart
                    {
                        CartId = foodItem.Object.CartId,
                        RestaurantName = foodItem.Object.RestaurantName,
                        RestaurantId = foodItem.Object.RestaurantId,
                        FoodId = foodItem.Object.FoodId,
                        FoodName = foodItem.Object.FoodName,
                        FoodDescription = foodItem.Object.FoodDescription,
                        FoodPrice = foodItem.Object.FoodPrice,
                        FoodRating = foodItem.Object.FoodRating,
                        RestaurantDescription = foodItem.Object.RestaurantDescription,
                        RestaurantAddress = foodItem.Object.RestaurantAddress,
                        RestaurantRating = foodItem.Object.RestaurantRating,
                        RestaurantDistance = foodItem.Object.RestaurantDistance,
                        FoodImage = foodItem.Object.FoodImage,
                        Quantity = foodItem.Object.Quantity,
                        RestaurantLat = foodItem.Object.RestaurantLat,
                        RestaurantLong = foodItem.Object.RestaurantLong

                    }));
                }
                return cartItemsList;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while fetching cart item" + ex.Message);
                return new List<AddFoodToCart>();
            }
        }
        public async Task<bool> DecrementFoodQuantityAsync(AddFoodToCart addFoodToCart)
        {
            try
            {
                var UserId = Preferences.Get("LocalId", string.Empty);
                var foodItems = await firebaseClient
                    .Child("CartItem")
                    .Child(UserId)
                    .Child(addFoodToCart.RestaurantId.ToString())
                    .OnceAsync<AddFoodToCart>();

                var foodItem = foodItems
                    .FirstOrDefault(item => item.Object.FoodId == addFoodToCart.FoodId);
                if (foodItem == null)
                {
                    return false;
                }

                var updatedFood = foodItem.Object;
                if (updatedFood.Quantity > 1)
                {
                    updatedFood.Quantity -= 1;
                    await firebaseClient
                        .Child("CartItem")
                        .Child(UserId)
                        .Child(addFoodToCart.RestaurantId.ToString())
                        .Child(foodItem.Key)
                        .PutAsync(updatedFood);
                }
                else
                {
                    await firebaseClient
                        .Child("CartItem")
                        .Child(UserId)
                        .Child(addFoodToCart.RestaurantId.ToString())
                        .Child(foodItem.Key)
                        .DeleteAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while decrement the value of food in cart" + ex.Message);
                return false;
            }
        }
        public async Task<bool> DeleteCartItemAsync(AddFoodToCart addFoodToCart)
        {
            try
            {
                var UserId = Preferences.Get("LocalId", string.Empty);
                var foodItems = await firebaseClient
                  .Child("CartItem")
                  .Child(UserId)
                  .Child(addFoodToCart.RestaurantId.ToString())
                  .OnceAsync<AddFoodToCart>();

                var foodItem = foodItems
                    .FirstOrDefault(item => item.Object.FoodId == addFoodToCart.FoodId);
                if (foodItem == null)
                {
                    return false;
                }
                else
                {
                    await firebaseClient
                      .Child("CartItem")
                      .Child(UserId)
                      .Child(addFoodToCart.RestaurantId.ToString())
                      .Child(foodItem.Key)
                      .DeleteAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while deleting the cart item" + ex.Message);
                return false;
            }
        }
        public async Task<bool> DeleteRestaurantToReplaceAync(string userId, int selectedRestaurantId)
        {
            try
            {
                var cartItems = await firebaseClient
                    .Child("CartItem")
                    .Child(userId)
                    .OnceAsync<AddFoodToCart>();

                var itemsToDelete = cartItems
                    .Where(item => item.Object.RestaurantId != selectedRestaurantId)
                    .ToList();
                foreach (var item in itemsToDelete)
                {
                    await firebaseClient
                        .Child("CartItem")
                        .Child(userId)
                        .Child(item.Key)
                        .DeleteAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while getting restaurand id " + ex.Message);
                return false;
            }
        }
        public async Task<UserDataModel> GetUserDataAsync(string userId)
        {
            try
            {
                var user = (await firebaseClient
                    .Child("UserData")
                    .Child(userId)
                    .OnceAsync<UserDataModel>())
                    .FirstOrDefault();
                return user?.Object;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while fetching user data " + ex.Message);
                return null;
            }
        }
        public async Task UpdateUserAddressAysnc(string userId, string newAddress)
        {
            try
            {
                Console.WriteLine($"Updating address for user: {userId} to {newAddress}");

                var userChildren = await firebaseClient
                    .Child("UserData")
                    .Child(userId)
                    .OnceAsync<UserDataModel>();
                var userNode = userChildren.FirstOrDefault();
                if (userNode != null)
                {
                    var thirdChildKey = userNode.Key;
                    Console.WriteLine($"Third child key: {thirdChildKey}");

                    var updates = new Dictionary<string, object>
                    {
                        { "Address", newAddress }
                    };
                    await firebaseClient
                        .Child("UserData")
                        .Child(userId)
                        .Child(thirdChildKey)
                        .PatchAsync(updates);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while updating user address" + ex.Message);
            }
        }
        public async Task<bool> IsDifferentRestaurantPresentAsync(string userId, int restaurantId)
        {
            try
            {
                var cartItemRef = firebaseClient.Child("CartItem").Child(userId);
                var snapshot = await cartItemRef.OnceAsync<Dictionary<string, object>>();

                if (snapshot == null || snapshot.Count == 0)
                {
                    // CartItem is empty
                    return false;
                }

                foreach (var restaurantSnapshot in snapshot)
                {
                    if (int.TryParse(restaurantSnapshot.Key, out int existingRestaurantId))
                    {
                        Console.WriteLine($"Existing Restaurant ID: {existingRestaurantId}, Provided Restaurant ID: {restaurantId}");

                        if (existingRestaurantId == restaurantId)
                        {
                            // Restaurant ID already exists
                            return false;
                        }
                    }
                }
                // Different restaurant ID found
                return true;
            }
            catch (FirebaseException fbEx)
            {
                Console.WriteLine("Firebase Exception: " + fbEx.Message);
                Console.WriteLine("Firebase StackTrace: " + fbEx.StackTrace);
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("***************** Exception: " + ex.Message);
                Console.WriteLine("***************** StackTrace: " + ex.StackTrace);
                return false;
            }
        }
        public async Task AddPaymentMethodAsync(string userId, PaymentModel paymentMethod)
        {
            try
            {
                var allUsers = await firebaseClient
                    .Child("UserData")
                    .Child(userId)
                    .OnceAsync<UserDataModel>();
                var userNode = allUsers.FirstOrDefault();
                if (userNode.Key != null)
                {
                    var userData = userNode.Object;
                    if (userData.PaymentMethod == null)
                    {
                        userData.PaymentMethod = new List<PaymentModel>();
                    }
                    userData.PaymentMethod.Add(paymentMethod);
                    await firebaseClient
                       .Child("UserData")
                       .Child(userId)
                       .Child(userNode.Key)
                       .PatchAsync(JsonConvert.SerializeObject(userData));
                    Console.WriteLine("Payment method added successfully.");
                }
                else
                {
                    Console.WriteLine("User not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while adding payment method " + ex.Message);
            }
        }
        public async Task<bool> PlacedOrderAsync(string UserId, List<OrderPlacedModel> selectedOrder)
        {
            try
            {
                var userOrderRef = firebaseClient.Child("PlacedOrder").Child(UserId);
                foreach (var order in selectedOrder)
                {
                    await userOrderRef.Child(order.OrderId.ToString()).PostAsync(order);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while plced order" + ex.Message);
                return false;
            }
        }
        public async Task<bool> DeleteCartByIdAsync(string userId, int? restaurantId)
        {
            try
            {
                await firebaseClient
                     .Child("CartItem")
                     .Child(userId)
                     .Child(restaurantId.ToString())
                     .DeleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while deleting cart item by id" + ex.Message);
                return false;
            }
        }
        public async Task<List<UserDataModel>> GetAllUser()
        {
            try
            {
                /* var users = await firebaseClient
                     .Child("UserData")
                     .OnceAsync<UserDataModel>();

                 var userList = new List<UserDataModel>();

                 foreach (var user in users)
                 {
                     var userId = user.Key;
                     var userDetails = await firebaseClient
                         .Child("UserData")
                         .Child(userId)
                         .OnceAsync<UserDataModel>();

                     foreach (var detail in userDetails)
                     {
                         var userData = detail.Object;
                         userData.Id = userId; 
                         userList.Add(userData);
                     }
                 }
                 return userList;*/

                var usersData = await firebaseClient
                    .Child("UserData")
                    .OnceSingleAsync<Dictionary<string, Dictionary<string, UserDataModel>>>();

                var userList = usersData
                .SelectMany(userEntry => userEntry.Value.Select(userDetail =>
                {
                    userDetail.Value.Id = userEntry.Key;
                    return userDetail.Value;
                }))
                .ToList();
                return userList;

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while getting all user details" + ex.Message);
                return null;
            }
        }
        public async Task LogoutAsync()
        {
            try
            {
                Preferences.Remove("IsLoggedIn");
                Preferences.Remove("FirebaseToken");
                Preferences.Remove("RefereshToken");
                Preferences.Remove("LocalId");
                Preferences.Clear();
                await Toast.Make("Successfully Logout").Show();
                Microsoft.Maui.Controls.Application.Current.MainPage = new NavigationPage(new LoginPage());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"*****************{ex.Message}");
            }
        }

        // Add food to cart from popular menu list
        public async Task<bool> AddFoodToCartFromPopularMenuAsync(AddFoodToCartByPopularMenu addFoodToCart)
        {
            try
            {
                addFoodToCart.UserId = Preferences.Get("LocalId", string.Empty);
                var firebaseClient = new FirebaseClient(DatabaseUrl);

                // Check if the item already exists in the cart (matching UserId, RestaurantId, and FoodId)
                var existingItems = await firebaseClient
                   .Child("CartItem")
                   .Child(addFoodToCart.UserId)
                   .Child(addFoodToCart.RestaurantId.ToString())
                   .OnceAsync<AddFoodToCart>();
                var existingItem = existingItems
                    .FirstOrDefault(item => item.Object.FoodId == addFoodToCart.FoodId);
                if (existingItem != null)
                {
                    // Item exists, update the quantity
                    var updatedFood = existingItem.Object;
                    updatedFood.Quantity += 1;
                    await firebaseClient
                        .Child("CartItem")
                        .Child(addFoodToCart.UserId)
                        .Child(addFoodToCart.RestaurantId.ToString())
                        .Child(existingItem.Key)
                        .PutAsync(updatedFood);
                    return true;
                }
                else
                {
                    // Item does not exist, add new
                    await firebaseClient
                        .Child("CartItem")
                        .Child(addFoodToCart.UserId)
                        .Child(addFoodToCart.RestaurantId.ToString())
                        .PostAsync(addFoodToCart);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while uploading detail for cart: {ex.Message}\nStack Trace: {ex.StackTrace}");
                await Toast.Make(ex.Message).Show();
                return false;
            }
        }
    }
}
