using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DatingApp.API.Controllers
{
  [Authorize]
  [Route("api/users/{userId}/photos")]
  public class PhotosController : Controller
  {
    private readonly IDatingRepository _datingRepository;
    private readonly IMapper _mapper;
    private readonly IOptions<CloudinarySettings> _cloudinarySettings;
    private Cloudinary _cloudinary;

    public PhotosController(IDatingRepository datingRepository, IMapper mapper, IOptions<CloudinarySettings> cloudinarySettings)
    {
      this._cloudinarySettings = cloudinarySettings;
      this._mapper = mapper;
      this._datingRepository = datingRepository;
      Account acc = new Account(
        _cloudinarySettings.Value.CloudName,
        _cloudinarySettings.Value.ApiKey,
        _cloudinarySettings.Value.ApiSecret
      );

      _cloudinary = new Cloudinary(acc);
    }

    [HttpGet("{id}", Name = "GetPhoto")]
    public async Task<IActionResult> GetPhoto(int id)
    {
      var photoFromRepo = await _datingRepository.GetPhoto(id);
      var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepo);
      return Ok(photo);
    }

    [HttpPost]
    public async Task<IActionResult> AddPhotoForUser(int userId, PhotoForCreationDto photoDto)
    {
      var user = await _datingRepository.GetUser(userId);
      if (user == null) return BadRequest("Could not find user");
      var currentUserID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

      if (currentUserID != user.Id) return Unauthorized();

      var file = photoDto.File;
      var uploadResult = new ImageUploadResult();
      if (file.Length > 0)
      {
        using (var stream = file.OpenReadStream())
        {
          var imageUploadParams = new ImageUploadParams()
          {
            File = new FileDescription(file.Name, stream)
          };
          uploadResult = _cloudinary.Upload(imageUploadParams);
        }
      }
      photoDto.Url = uploadResult.Uri.ToString();
      photoDto.PublicId = uploadResult.PublicId;

      var photo = _mapper.Map<Photo>(photoDto);
      photo.User = user;
      if (!user.Photos.Any(p => p.IsMain)) photo.IsMain = true;

      user.Photos.Add(photo);

      var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);
      if (await _datingRepository.SaveAll()) return CreatedAtRoute("GetPhoto", new { id = photo.Id }, photoToReturn);

      return BadRequest("Could not add the photo ${photo.Name}");
    }
  }
}