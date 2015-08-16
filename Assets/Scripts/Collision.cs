using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// The Collision component is a component which throw events when a collision occurs at its parent level
public class Collision : MonoBehaviour
{

    // Only what is tagged with TAG will be taken into account
    public static readonly string TAG = "Collisionable";

    // We can register to OnCollided to carry the collisions
    public delegate void CollisionAction(GameObject collider);
    public event CollisionAction OnCollided;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // First of all we check to be sure that we have a Sprite.
        Sprite sprite = gameObject.GetComponent<Sprite>();
        // If we don't have one, then we won't intersect anything.
        if (sprite == null)
            return;

        // STEP 1 : Get only the interesting objects (tag)
        List<GameObject> objects = new List<GameObject>(GameObject.FindGameObjectsWithTag(TAG));

        // We will then use rectangles overlap, so let's create those
        Rect spriteRectangle = new Rect(gameObject.transform.position.x,
            // Sprite start from bottom left but Rectangles start from top left
                                         gameObject.transform.position.y + sprite.SpriteSize.y,
                                         sprite.SpriteSize.x,
                                         sprite.SpriteSize.y);

        Rect otherSpriteRectangle = new Rect();
        Sprite otherSprite;

        List<QuadTreePair> items = new List<QuadTreePair>();
        items.Add(new QuadTreePair(gameObject, spriteRectangle));

        foreach (var obj in objects)
        {

            // We check that the object we are looking at is not us, we always collide with us.
            if (obj == gameObject)
                continue;

            // We retrieve the sprite of the other object
            otherSprite = obj.GetComponent<Sprite>();

            // Why would we check for collision if it doesn't have a sprite anyway?
            if (otherSprite == null)
                continue;

            // We create the second rectangle, same thing as our own rectangle
            otherSpriteRectangle.x = obj.transform.position.x;
            otherSpriteRectangle.y = obj.transform.position.y + sprite.SpriteSize.y;
            otherSpriteRectangle.width = otherSprite.SpriteSize.x;
            otherSpriteRectangle.height = otherSprite.SpriteSize.y;

            // We prepare the list we will send to the QuadTree
            items.Add(new QuadTreePair(obj, otherSpriteRectangle));

        }

        // We request the creation of the quadTree from the manager. If it already exists, it won't be created.
        QuadTree<QuadTreePair> quadTree;
        QuadTreeManager<QuadTreePair> manager = new QuadTreeManager<QuadTreePair>();
        // We push everything into the tree, it is returned after the changed
        quadTree = manager.pushIntoTree(items);

        // We request the sprite which can be in conflict with us
        List<QuadTreePair> list = quadTree.Query(spriteRectangle);

        foreach (var pair in list)
        {
            // We check that the object we are looking at is not us, we always collide with us.
            if (pair.gameObject == gameObject)
                continue;

            // STEP 3 : Rectangles
            // Overlap is exactly what we want to use, let's use it then
            if (pair.rectangle.Overlaps(spriteRectangle))
            {

                // As with every delegate, we check if it's null before using it then we use it.
                if (OnCollided != null)
                    // Our event will be launch, the gameObject owning this component should consider listening to it.
                    OnCollided(pair.gameObject);
            }
        }
    }
}
