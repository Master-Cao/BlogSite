using Site.Domain.Entities;
using SqlSugar;
using YjSite.DTOs;

namespace YjSite.Services.DefaultImageService
{
    public class DefaultImageService : IDefaultImageService
    {
        private readonly ISqlSugarClient _db;
        private readonly ILogger<DefaultImageService> _logger;

        public DefaultImageService(ISqlSugarClient db, ILogger<DefaultImageService> logger)
        {
            _db = db;
            _logger = logger;
        }

        /// <summary>
        /// 获取默认图片列表
        /// </summary>
        public async Task<(List<DefaultImageResponse>, int)> GetDefaultImagesAsync(int page, int pageSize)
        {
            try
            {
                var total = await _db.Queryable<DefaultImage>()
                    .Where(img => !img.IsDeleted)
                    .CountAsync();

                var images = await _db.Queryable<DefaultImage>()
                    .Where(img => !img.IsDeleted)
                    .OrderByDescending(img => img.CreateTime)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(img => new DefaultImageResponse
                    {
                        Id = img.Id,
                        Url = img.Url,
                        CreateUserId = img.CreateUserId,
                        CreateTime = img.CreateTime
                    })
                    .ToListAsync();

                return (images, total);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取默认图片列表失败");
                throw;
            }
        }

        /// <summary>
        /// 根据ID获取默认图片
        /// </summary>
        public async Task<DefaultImageResponse> GetDefaultImageByIdAsync(string id)
        {
            try
            {
                var image = await _db.Queryable<DefaultImage>()
                    .Where(img => img.Id == id && !img.IsDeleted)
                    .Select(img => new DefaultImageResponse
                    {
                        Id = img.Id,
                        Url = img.Url,
                        CreateUserId = img.CreateUserId,
                        CreateTime = img.CreateTime
                    })
                    .FirstAsync();

                return image;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取默认图片失败，ID: {id}");
                return null;
            }
        }

        /// <summary>
        /// 创建默认图片
        /// </summary>
        public async Task<DefaultImageResponse> CreateDefaultImageAsync(CreateDefaultImageRequest request, string userId)
        {
            try
            {
                var image = new DefaultImage
                {
                    Url = request.Url,
                    CreateUserId = userId,
                    CreateTime = DateTime.Now
                };

                await _db.Insertable(image).ExecuteCommandAsync();

                return new DefaultImageResponse
                {
                    Id = image.Id,
                    Url = image.Url,
                    CreateUserId = image.CreateUserId,
                    CreateTime = image.CreateTime
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建默认图片失败");
                return null;
            }
        }

        /// <summary>
        /// 更新默认图片
        /// </summary>
        public async Task<DefaultImageResponse> UpdateDefaultImageAsync(string id, UpdateDefaultImageRequest request, string userId)
        {
            try
            {
                var image = await _db.Queryable<DefaultImage>()
                    .Where(img => img.Id == id && !img.IsDeleted)
                    .FirstAsync();

                if (image == null)
                {
                    return null;
                }

                image.Url = request.Url;

                await _db.Updateable(image).ExecuteCommandAsync();

                return new DefaultImageResponse
                {
                    Id = image.Id,
                    Url = image.Url,
                    CreateUserId = image.CreateUserId,
                    CreateTime = image.CreateTime
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"更新默认图片失败，ID: {id}");
                return null;
            }
        }

        /// <summary>
        /// 删除默认图片
        /// </summary>
        public async Task<bool> DeleteDefaultImageAsync(string id, string userId)
        {
            try
            {
                var image = await _db.Queryable<DefaultImage>()
                    .Where(img => img.Id == id && !img.IsDeleted)
                    .FirstAsync();

                if (image == null)
                {
                    return false;
                }

                image.IsDeleted = true;
                image.DeleteUserId = userId;
                image.DeleteTime = DateTime.Now;

                await _db.Updateable(image).ExecuteCommandAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"删除默认图片失败，ID: {id}");
                return false;
            }
        }
    }
} 