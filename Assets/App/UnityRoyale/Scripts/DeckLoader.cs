﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace UnityRoyale
{
    public class DeckLoader : MonoBehaviour
    {
        private DeckData targetDeck;
        public UnityAction OnDeckLoaded;

        public void LoadDeck(DeckData deckToLoad)
        {
            targetDeck = deckToLoad;
            Addressables.LoadAssets<CardData>(targetDeck.labelsToInclude[0].labelString, null).Completed += obj => {
                    targetDeck.CardsRetrieved((List<CardData>)obj.Result);

                    if(OnDeckLoaded != null)
                        OnDeckLoaded();

                    Destroy(this); 
            };
        }

        //...

		// private void OnResourcesRetrieved(IAsyncOperation<IList<CardData>> obj)
		// {

		// }
	}
}