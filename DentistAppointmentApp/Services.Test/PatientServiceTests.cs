namespace DentistApp.Services.UnitTest
{
    using DentistApp.Data.Models;
    using DentistApp.Data.Repositories.Contracts;
    using static DentistApp.GCommon.Roles;

    using DentistApp.Services.Core;
    using DentistApp.Services.Core.Contracts;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using NSubstitute;
    public class PatientServiceTests
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IPatientRepository patientRepository;

        private readonly PatientService patientService;

        public PatientServiceTests()
        {
            IUserStore<ApplicationUser> userStore = Substitute.For<IUserStore<ApplicationUser>>();
            IOptions<IdentityOptions> options = Substitute.For<IOptions<IdentityOptions>>();
            IPasswordHasher<ApplicationUser> passwordHasher = Substitute.For<IPasswordHasher<ApplicationUser>>();
            IUserValidator<ApplicationUser>[] userValidators = [];
            IPasswordValidator<ApplicationUser>[] passwordValidators = [];
            ILookupNormalizer keyNormalizer = Substitute.For<ILookupNormalizer>();
            IdentityErrorDescriber errors = new IdentityErrorDescriber();
            IServiceProvider services = Substitute.For<IServiceProvider>();
            ILogger<UserManager<ApplicationUser>> logger = Substitute.For<ILogger<UserManager<ApplicationUser>>>();

            options.Value.Returns(new IdentityOptions());

            userManager = Substitute.For<UserManager<ApplicationUser>>(
                userStore,
                options,
                passwordHasher,
                userValidators,
                passwordValidators,
                keyNormalizer,
                errors,
                services,
                logger);

            patientRepository = Substitute.For<IPatientRepository>();

            patientService = new PatientService(userManager, patientRepository);
        }

        [Fact]
        public async Task GetDentistIdAsync_ShouldReturnFirstDentistId_WhenDentistExists()
        {
            // Arrange
            IList<ApplicationUser> dentists = new List<ApplicationUser>
            {
                 new ApplicationUser { Id = "dentist-1" }
            };

            userManager
                .GetUsersInRoleAsync(DentistRoleName)
                .Returns(dentists);

            // Act
            string? result = await patientService.GetDentistIdAsync();

            // Assert
            Assert.Equal("dentist-1", result);
        }

        [Fact]
        public async Task GetDentistIdAsync_ShouldReturnNull_WhenNoDentistsExist()
        {
            // Arrange
            IList<ApplicationUser> dentists = new List<ApplicationUser>();

            userManager
                .GetUsersInRoleAsync(DentistRoleName)
                .Returns(Task.FromResult(dentists));

            // Act
            string? result = await patientService.GetDentistIdAsync();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task IsUserDentistByIdAsync_ShouldReturnFalse_WhenUserDoesNotExist()
        {
            // Arrange
            string userId = "missing-user";

            userManager
                .FindByIdAsync(userId)
                .Returns((ApplicationUser?)null);

            // Act
            bool result = await patientService.IsUserDentistByIdAsync(userId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsUserDentistByIdAsync_ShouldReturnTrue_WhenUserIsDentist()
        {
            // Arrange
            string userId = "dentist-1";
            ApplicationUser user = new ApplicationUser
            {
                Id = userId
            };

            userManager
                .FindByIdAsync(userId)
                .Returns(user);

            userManager
                .IsInRoleAsync(user, DentistRoleName)
                .Returns(true);

            // Act
            bool result = await patientService.IsUserDentistByIdAsync(userId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsUserDentistByIdAsync_ShouldReturnFalse_WhenUserIsNotDentist()
        {
            // Arrange
            string userId = "user-1";
            ApplicationUser user = new ApplicationUser
            {
                Id = userId
            };

            userManager
                .FindByIdAsync(userId)
                .Returns(user);

            userManager
                .IsInRoleAsync(user, DentistRoleName)
                .Returns(false);

            // Act
            bool result = await patientService.IsUserDentistByIdAsync(userId);

            // Assert
            Assert.False(result);
        }
    }
}
