using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CommentScrollView : MonoBehaviour
{
    
    [SerializeField] private CommentSectionController _commentSectionController; 
    [SerializeField] private Transform _commentSectionParent; 
    [SerializeField] private GameObject _commentTextExample;
    private List < GameObject > _commentsList = new List < GameObject >();
    private int _commentsCount = 0;


    private void Awake()
    {
        _commentSectionController.CommentUploadEvent.AddListener(AddCommentToScrollView);
        _commentSectionController.CommentUploadEvent.AddListener(AddCommentToPlayerPrefs);
        
        _commentsCount = PlayerPrefs.GetInt( "CommentsCount" );
        
        for( int i = 0; i < _commentsCount; i++ )
        {
            string comment =   PlayerPrefs.GetString( "Comment" + i);
            if( comment != "" )
            {
                GameObject instantiate = Instantiate( _commentTextExample, _commentSectionParent );
                instantiate.GetComponent < TextMeshProUGUI >().text = comment;
                instantiate.SetActive( true );
                _commentsList.Add( instantiate );
            }
        }
    }
    
    public void AddCommentToScrollView(string inputText)
    {
        GameObject instance = Instantiate( _commentTextExample, _commentSectionParent );
        instance.SetActive( true );
        instance.transform.GetComponent < TextMeshProUGUI >().text = DateTime.Now + ": " + inputText;
        _commentsList.Add( instance );
    }

    public void AddCommentToPlayerPrefs( string inputText )
    {
        ++_commentsCount;
        int commentNumber = _commentsCount - 1;
        PlayerPrefs.SetString( "Comment" + commentNumber, DateTime.Now + ": " + inputText );
        PlayerPrefs.SetInt( "CommentsCount", _commentsCount);
        Debug.Log( "Comment Added: " + inputText );
    }

    public void DeleteAllComments()
    {
        PlayerPrefs.DeleteAll();

        foreach( var comment in _commentsList )
        {
            Destroy( comment );
        }
        _commentsList.Clear();
        _commentsCount = 0;
    }

    private void OnDestroy()
    {
        _commentSectionController.CommentUploadEvent.RemoveListener(AddCommentToScrollView);
        _commentSectionController.CommentUploadEvent.RemoveListener(AddCommentToPlayerPrefs);
    }
}
