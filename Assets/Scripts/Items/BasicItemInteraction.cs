using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Interactable))]
public class BasicItemInteraction : MonoBehaviour
{
    public Item itemProperties;

    private Interactable interactable;

    private Vector3 oldPosition;
    private Quaternion oldRotation;

    private float attachTime;

    private Hand.AttachmentFlags attachmentFlags = Hand.defaultAttachmentFlags & (~Hand.AttachmentFlags.SnapOnAttach) & (~Hand.AttachmentFlags.DetachOthers) & (~Hand.AttachmentFlags.VelocityMovement);

    private void Start()
    {
        
    }

    void Awake()
    {
        interactable = GetComponent<Interactable>();
        
    }

    //-------------------------------------------------
    // Called every Update() while a Hand is hovering over this object
    //-------------------------------------------------
    private void HandHoverUpdate(Hand hand)
    {
        GrabTypes startingGrabType = hand.GetGrabStarting();
        bool isGrabEnding = hand.IsGrabEnding(this.gameObject);

        if (interactable.attachedToHand == null && startingGrabType != GrabTypes.None && GlobalContext.Instance.GetContextValue("item_held").Equals("false"))
        {
            // Save our position/rotation so that we can restore it when we detach
            oldPosition = transform.position;
            oldRotation = transform.rotation;

            // Call this to continue receiving HandHoverUpdate messages,
            // and prevent the hand from hovering over anything else
            hand.HoverLock(interactable);

            // Attach this object to the hand
            hand.AttachObject(gameObject, startingGrabType, attachmentFlags);
        }
        else if (isGrabEnding)
        {
            // Detach this object from the hand
            hand.DetachObject(gameObject);

            // Call this to undo HoverLock
            hand.HoverUnlock(interactable);

            // Restore position/rotation
            transform.position = oldPosition;
            transform.rotation = oldRotation;

            KeyValue kv;
            kv.key = "item_held";
            kv.value = "false";
            kv.type = KeyValueType.Bool;
            GlobalContext.Instance.SetContext(kv);
        }
    }

    //-------------------------------------------------
    // Called when this GameObject becomes attached to the hand
    //-------------------------------------------------
    private void OnAttachedToHand(Hand hand)
    {
        if (itemProperties != null)
        {
            KeyValue kv1, kv2, kv3;
            // Tell IBM an item is held
            kv1.key = "item_held";
            kv1.value = "true";
            kv1.type = KeyValueType.Bool;
            GlobalContext.Instance.SetContext(kv1);
            // Transmit to bot the name of the item held
            kv2.key = "item_name";
            kv2.value = itemProperties.name;
            kv2.type = KeyValueType.String;
            GlobalContext.Instance.SetContext(kv2);
            // Transmit a short description of the purpose of the item
            kv3.key = "item_description";
            kv3.value = itemProperties.description;
            kv3.type = KeyValueType.String;
            GlobalContext.Instance.SetContext(kv3);
            attachTime = Time.time;
        }
        
    }


    private bool lastHovering = false;
    private void Update()
    {
        if (interactable.isHovering != lastHovering) //save on the .tostrings a bit
        {
            lastHovering = interactable.isHovering;
        }
    }
}
