﻿using System.Collections.Generic;

namespace GowBoard.Models.DTO.RequestDTO
{
    public class ReqBoardDTO
    {
        public int BoardContentId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public List<int> BoardFileId { get; set; }
    }
}