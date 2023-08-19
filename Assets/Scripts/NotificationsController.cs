using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NotificationSamples;

public class NotificationsController : MonoBehaviour
{
    private GameNotificationsManager notificationsManager;

    private static bool addedReminder = false;

    // Start is called before the first frame update
    void Start()
    {
        //Get access to the notifications manager
        notificationsManager = GetComponent<GameNotificationsManager>();

        //Create a channel to use for it (required for android)
        GameNotificationChannel channel = new GameNotificationChannel(
            "channel0",
            "Default Channel",
            "Generic Notifications"
        );

        //Initialize the manager so it can be used
        StartCoroutine(notificationsManager.Initialize(channel));

        //Check if the notification hasnt been added yet
        if (!addedReminder)
        {
            //Remind the player to come back tomorrow to play the game
            ShowNotification(
                "Endless Runner",
                "Come back and try to beat your score!!",
                DateTime.Now.AddDays(1)
            );

            //Cannot add again until the user comes back
            addedReminder = true;
        }
    }

    public void ShowNotification(string title, string body, DateTime deliveryTime)
    {
        IGameNotification notification = notificationsManager.CreateNotification();

        if (notification != null)
        {
            notification.Title = title;
            notification.Body = body;
            notification.DeliveryTime = deliveryTime;
            notification.SmallIcon = "icon_0";
            notification.LargeIcon = "icon_1";
            notification.BadgeNumber = 5;

            notificationsManager.ScheduleNotification(notification);
        }

        Debug.Log("Sending notification: " + title);
    }
}
