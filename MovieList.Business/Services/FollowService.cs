using MovieList.Business.Interfaces;
using MovieList.Core.DTOs.User;
using MovieList.Core.Entities;
using MovieList.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieList.Business.Services
{
    public class FollowService : IFollowService
    {
        private readonly IFollowRepository _followRepository;
        private readonly IUserRepository _userRepository;

        public FollowService(
            IFollowRepository followRepository,
            IUserRepository userRepository)
        {
            _followRepository = followRepository;
            _userRepository = userRepository;
        }

        public async Task<bool> FollowUserAsync(int followerId, int followingId)
        {
            // Kendini takip edemez
            if (followerId == followingId)
                return false;

            // Zaten takip ediyor mu?
            var isFollowing = await _followRepository.IsFollowingAsync(followerId, followingId);
            if (isFollowing)
                return false;

            // Takip et
            var follow = new Follow
            {
                FollowerId = followerId,
                FollowingId = followingId,
                CreatedDate = DateTime.UtcNow
            };

            await _followRepository.AddAsync(follow);

            // Kullanıcıların follower/following sayılarını güncelle
            await UpdateFollowCountsAsync(followerId, followingId);

            return true;
        }

        public async Task<bool> UnfollowUserAsync(int followerId, int followingId)
        {
            var follow = await _followRepository.GetFollowAsync(followerId, followingId);

            if (follow == null)
                return false;

            await _followRepository.DeleteAsync(follow);

            // Kullanıcıların follower/following sayılarını güncelle
            await UpdateFollowCountsAsync(followerId, followingId);

            return true;
        }

        public async Task<bool> IsFollowingAsync(int followerId, int followingId)
        {
            return await _followRepository.IsFollowingAsync(followerId, followingId);
        }

        public async Task<int> GetFollowerCountAsync(int userId)
        {
            return await _followRepository.GetFollowerCountAsync(userId);
        }

        public async Task<int> GetFollowingCountAsync(int userId)
        {
            return await _followRepository.GetFollowingCountAsync(userId);
        }

        private async Task UpdateFollowCountsAsync(int followerId, int followingId)
        {
            // Takip eden kullanıcının following count'u
            var follower = await _userRepository.GetByIdAsync(followerId);
            if (follower != null)
            {
                follower.FollowingCount = await _followRepository.GetFollowingCountAsync(followerId);
                await _userRepository.UpdateAsync(follower);
            }

            // Takip edilen kullanıcının follower count'u
            var following = await _userRepository.GetByIdAsync(followingId);
            if (following != null)
            {
                following.FollowerCount = await _followRepository.GetFollowerCountAsync(followingId);
                await _userRepository.UpdateAsync(following);
            }
        }
        public async Task<IEnumerable<UserDto>> GetFollowersAsync(int userId)
        {
            var followers = await _followRepository.GetFollowersAsync(userId);

            return followers.Select(f => new UserDto
            {
                Id = f.Follower.Id,
                Username = f.Follower.UserName ?? f.Follower.Email,
                Bio = f.Follower.Bio,
                ProfileImageUrl = f.Follower.ProfilePictureUrl
            });
        }

        public async Task<IEnumerable<UserDto>> GetFollowingAsync(int userId)
        {
            var following = await _followRepository.GetFollowingAsync(userId);

            return following.Select(f => new UserDto
            {
                Id = f.Following.Id,
                Username = f.Following.UserName ?? f.Following.Email,
                Bio = f.Following.Bio,
                ProfileImageUrl = f.Following.ProfilePictureUrl
            });
        }
    }
}