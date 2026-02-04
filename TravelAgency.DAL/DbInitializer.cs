using Microsoft.AspNetCore.Identity;
using TravelAgency.BLL.Entities;
using TravelAgency.DAL.Data;

namespace TravelAgency.DAL
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            string adminEmail,
            string adminPassword,
            string adminFirstName,
            string adminLastName)
        {
            // Создаем роли
            string[] roleNames = { "Admin", "Manager", "Client" };
            
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
            
            // Создаем администратора
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = adminFirstName,
                    LastName = adminLastName,
                    EmailConfirmed = true
                };
                
                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                    await userManager.AddToRoleAsync(adminUser, "Manager");
                }
            }

            if (!context.Countries.Any())
            {
                var countries = new List<Country>
                {
                    new Country { Name = "Турция", Description = "Страна с богатой историей и прекрасными пляжами" },
                    new Country { Name = "Египет", Description = "Земля фараонов и Красного моря" },
                    new Country { Name = "Греция", Description = "Колыбель европейской цивилизации" },
                    new Country { Name = "Италия", Description = "Страна искусства, моды и кухни" },
                    new Country { Name = "Испания", Description = "Солнечная страна с богатой культурой" }
                };
                
                context.Countries.AddRange(countries);
                await context.SaveChangesAsync();
            }
            
            // Тестовые данные для городов
            if (!context.Cities.Any())
            {
                var cities = new List<City>
                {
                    new City { Name = "Анталия", CountryId = 1 },
                    new City { Name = "Стамбул", CountryId = 1 },
                    new City { Name = "Хургада", CountryId = 2 },
                    new City { Name = "Шарм-эль-Шейх", CountryId = 2 },
                    new City { Name = "Афины", CountryId = 3 },
                    new City { Name = "Родос", CountryId = 3 },
                    new City { Name = "Рим", CountryId = 4 },
                    new City { Name = "Милан", CountryId = 4 },
                    new City { Name = "Барселона", CountryId = 5 },
                    new City { Name = "Мадрид", CountryId = 5 }
                };
                
                context.Cities.AddRange(cities);
                await context.SaveChangesAsync();
            }
            
            // Тестовые данные для категорий отелей
            if (!context.HotelCategories.Any())
            {
                var categories = new List<HotelCategory>
                {
                    new HotelCategory { Name = "3 звезды", Stars = 3 },
                    new HotelCategory { Name = "4 звезды", Stars = 4 },
                    new HotelCategory { Name = "5 звезд", Stars = 5 }
                };
                
                context.HotelCategories.AddRange(categories);
                await context.SaveChangesAsync();
            }
            
            // Тестовые данные для типов туров
            if (!context.TourTypes.Any())
            {
                var types = new List<TourType>
                {
                    new TourType { Name = "Пляжный отдых" },
                    new TourType { Name = "Экскурсионный" },
                    new TourType { Name = "Шопинг" }
                };
                
                context.TourTypes.AddRange(types);
                await context.SaveChangesAsync();
            }
            
            // Тестовые данные для туров
            if (!context.Tours.Any() && adminUser != null)
            {
                var tours = new List<Tour>
                {
                    new Tour
                    {
                        Title = "Отдых в Анталии 5*",
                        Description = "Прекрасный отдых в отеле 5 звезд с системой 'все включено'",
                        CountryId = 1,
                        CityId = 1,
                        HotelCategoryId = 3,
                        TourTypeId = 1,
                        DurationDays = 7,
                        Price = 85000,
                        MaxPeopleCount = 30,
                        StartDate = DateTime.Now.AddDays(14),
                        ImageUrlsJson = "[\"/images/turkey1.jpg\"]",
                        CreatedById = adminUser.Id,
                        IsActive = true
                    },
                    new Tour
                    {
                        Title = "Экскурсии по Риму",
                        Description = "Знакомство с историей и культурой Древнего Рима",
                        CountryId = 4,
                        CityId = 7,
                        HotelCategoryId = 2,
                        TourTypeId = 2,
                        DurationDays = 5,
                        Price = 65000,
                        MaxPeopleCount = 20,
                        StartDate = DateTime.Now.AddDays(21),
                        ImageUrlsJson = "[\"/images/rome1.jpg\"]",
                        CreatedById = adminUser.Id,
                        IsActive = true
                    }
                };
                
                context.Tours.AddRange(tours);
                await context.SaveChangesAsync();
            }
            if (!context.TourTypes.Any())
            {
                var tourTypes = new List<TourType>
                    {
                        new TourType { Name = "Пляжный отдых" },
                        new TourType { Name = "Экскурсионный" },
                        new TourType { Name = "Горнолыжный" },
                        new TourType { Name = "Лечебный" },
                        new TourType { Name = "Шопинг-тур" }
                    };

                context.TourTypes.AddRange(tourTypes);
                await context.SaveChangesAsync();
            }
        }
    }
}