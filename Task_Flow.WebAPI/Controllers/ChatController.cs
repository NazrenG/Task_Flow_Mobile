﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using Task_Flow.Business.Abstract;
using Task_Flow.DataAccess.Abstract;
using Task_Flow.Entities.Models;
using Task_Flow.WebAPI.Dtos;
//using Task_Flow.WebAPI.Hubs;

namespace Task_Flow.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IFriendService friendService;
        private readonly IChatMessageService chatMessageService;
        private readonly IChatService chatService;
        //private readonly IHubContext<ConnectionHub> hubContext;

        public ChatController(IUserService userService, IFriendService friendService, IChatService chatService, IChatMessageService chatMessageService)
        {
            this.userService = userService;
            this.friendService = friendService;
            //this.hubContext = hubContext;
            this.chatService = chatService;
            this.chatMessageService = chatMessageService;
        }

        [Authorize]
        [HttpGet("AllChatsWithFriends")]//+
        public async Task<IActionResult> GetAllFriends()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "error.userNotAuth" });
            }

            var myfriends = await friendService.GetFriends(userId);
            //var sortFriend=myfriends.Select(f=>f.)
            var sortFriend = new List<Friend>();
            foreach (var friend in myfriends) {
                var result = await friendService.MutualFriends(friend.UserId, friend.UserFriendId);
            if (result)
            sortFriend.Add(friend); 
            }

            var sorted = new List<FriendForMessageDto>();


            foreach (var item in sortFriend)
            {
                var chat = await chatService.GetByRecieverAndSenderId(userId, item.UserFriendId);
                ChatMessage latestmessage = null;
                bool isReciever = false;
                if (chat != null)
                {
                    latestmessage = await chatMessageService.GetLatestMessageByChatIdAsync(chat.Id);
                    isReciever =latestmessage!=null? latestmessage.SenderId !=userId:false;
                }
                var user = await userService.GetUserById(item.UserFriendId);
                sorted.Add(new FriendForMessageDto
                {
                   
                    RecentMessageDate = (latestmessage == null ? "" : latestmessage.SentDate.ToString()),
                    FriendFullname = user.Firstname + " " + user.Lastname,
                    FriendEmail = user.Email,
                    FriendImg = user.Image,
                    isReciever = isReciever,
                    RecentMessage = (latestmessage==null?"":latestmessage.Content),
                    IsOnline=user.IsOnline
                });

            }

            sorted =  sorted.OrderBy(x => x.isReciever).ToList();

            return Ok(new { List = sorted });


        }
       

        [Authorize]
        [HttpGet("UserMessages")]
        public async Task<IActionResult> GetUserMessages()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "error.userNotAuth" });
            }
            var chats =await chatService.GetAllChatByUserId(userId);
            if (chats != null)
            {
                var list = new List<AllMessagesDto>();
                foreach (var item in chats)
                {
                    ChatMessage lastmessage = null;

                    lastmessage = await chatMessageService.GetLatestMessageByChatIdAsync(item.Id);
                   var isReciever = lastmessage != null ? lastmessage.SenderId != userId : false;
                    if (isReciever)
                    {
                        var friend = await userService.GetUserById(lastmessage.SenderId);
                      
                        var data = new AllMessagesDto
                        {
                            FriendName=friend.Firstname,
                            FriendLastname=friend.Lastname,
                            FriendImg=friend.Image,
                            Message=lastmessage.Content,
                            SentDate=lastmessage.SentDate.ToShortDateString(),

                        };
                        list.Add(data);
                    }

                   
                }
                return Ok(new {Result=true,List=list});

            }
            return Ok(new {Resut=false});

        }


    }
}
