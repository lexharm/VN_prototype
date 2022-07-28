using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace VNCreator
{
    public class DisplayBase : MonoBehaviour
    {
        public StoryObject story;

        protected NodeData currentNode;
        protected bool lastNode;
        protected bool miniGame;

        protected List<string> loadList = new List<string>();

        void Awake()
        {
            if (PlayerPrefs.GetString(GameSaveManager.currentLoadName) == string.Empty)
            {
                currentNode = story.GetFirstNode();
                loadList.Add(currentNode.guid);
            }
            else
            {
                loadList = GameSaveManager.Load();
                if(loadList == null || loadList.Count == 0)
                {
                    currentNode = story.GetFirstNode();
                    loadList = new List<string>();
                    loadList.Add(currentNode.guid);
                }
                else
                {
                    currentNode = story.GetCurrentNode(loadList[loadList.Count - 1]);
                }
            }
        }

        protected virtual void NextNode(int _choiceId)
        {
            currentNode = story.GetNextNode(currentNode.guid, _choiceId);
            if (!lastNode) 
            {
                lastNode = currentNode.endNode;
                loadList.Add(currentNode.guid);
            }
            if (!miniGame)
            {
                miniGame = currentNode.miniGame;
            }
        }

        protected virtual void Previous()
        {
            loadList.RemoveAt(loadList.Count - 1);
            currentNode = story.GetCurrentNode(loadList[loadList.Count - 1]);
            lastNode = currentNode.endNode;
        }

        protected void Save()
        {
            GameSaveManager.Save(loadList);
        }
    }
}
