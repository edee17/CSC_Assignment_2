﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Stripe;
using TheLifeTimeTalents.Data;
using TheLifeTimeTalents.Services;

namespace TheLifeTimeTalents.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IConfiguration _configuration;
        private IUserService _userService;
        public ApplicationDbContext _database { get; }

        public PaymentController(IUserService userService, IConfiguration configuration, ApplicationDbContext database)
        {
            _userService = userService;
            _configuration = configuration;
            _database = database;
        }

        //public IActionResult Subscribe()
        //{
        //    ViewBag.stripeKey = _configuration["Stripe:publishable_key"];
        //    return View();
        //}
        [HttpPost]
        public IActionResult Upgrade(string subKey, string plan)
        {
            // Set your secret key. Remember to switch to your live secret key in production!
            // See your keys here: https://dashboard.stripe.com/account/apikeys
            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];

            var service = new SubscriptionService();
            Subscription subscription = service.Get(subKey);

            var planId = _configuration["Stripe:Daily10"];

            var items = new List<SubscriptionItemOptions> {
                new SubscriptionItemOptions {
                    Id = subscription.Items.Data[0].Id,
                    Price = planId,
                },
            };

            var options = new SubscriptionUpdateOptions
            {
                CancelAtPeriodEnd = false,
                ProrationBehavior = "create_prorations",
                Items = items,
            };
            subscription = service.Update(subKey, options);

            return View("UpdateResult");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Subscribe(string cardEmail, string plan, string stripeToken)
        {
            var customerOptions = new CustomerCreateOptions
            {
                Email = cardEmail,
                Source = stripeToken,
            };

            var customerService = new CustomerService();
            var customer = customerService.Create(customerOptions);

            var planId = _configuration["Stripe:Daily10"];

            var subscriptionOptions = new SubscriptionCreateOptions
            {
                Customer = customer.Id,
                Items = new List<SubscriptionItemOptions>
                {
                    new SubscriptionItemOptions
                    {
                        Plan = planId
                    },
                },
            };
            subscriptionOptions.AddExpand("latest_invoice.payment_intent");

            var subscriptionService = new SubscriptionService();
            var subscription = subscriptionService.Create(subscriptionOptions);

            ViewBag.stripeKey = _configuration["Stripe:PublishableKey"];
            ViewBag.subscription = subscription.ToJson();

            try
            {
                var user = _userService.UpdatePaidRole(cardEmail);

            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }

            return View("SubscribeResult");
        }

        
    }
}

