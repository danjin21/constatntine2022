using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApiData.Models;
using WebApi.Data;

namespace WebApi.Controllers
{
	// REST (Representational State Transfer)
	// 공식 표준 스펙은 아님
	// 원래 있던 HTTP 통신에서 기능을 '재사용'해서 데이터 송수신 규칙을 만든 것

	// CRUD
	// www.naver.com -> 현대 백화점 지하 1층
	// www.naver.com/sports/ -> 현대 백화점 지하 1층 일식당 XXX
	// verb (GET POST PUT ... )

	// Create
	// POST /api/ranking
	// -- 아이템 생성 요청 (Body에 실제 정보)

	// Read
	// GET /api/ranking
	// 모든 아이템 주세요
	// GET /api/ranking/1
	// id=1번인 아이템 주세요

	// Update
	// PUT /api/ranking (PUT 보안 문제로 웹에선 활용 X)
	// 아이템 갱신 요청 (Body에 실제 정보)

	// Delete
	// DELETE /api/ranking/1 (DELETE 보안 문제로 웹에서 활용 X)
	// id=1번인 아이템 삭제

	// ApiController 특징
	// 그냥 C# 객체를 반환해도 된다
	// null 반환하면 -> 클라에 204 Response (No Content)
	// string -> text/plain
	// 나머지 (int, bool) -> application/json

	[Route("api/[controller]")]
	[ApiController]
	public class BoardController : ControllerBase
	{
		ApplicationDbContext _context;

		public BoardController(ApplicationDbContext context)
		{
			_context = context;
		}

		// Create
		[HttpPost]
		public BoardContent AddBoardContent([FromBody] BoardContent boardContent)
		{
			_context.BoardContents.Add(boardContent);
			_context.SaveChanges();

			return boardContent;
		}

		// Read
		[HttpGet]
		public List<BoardContent> GetBoardContentList()
		{
			List<BoardContent> boardContents = _context.BoardContents
				//.OrderByDescending(item => item.Id)
				.OrderBy(item => item.Id)
				.ToList();

			return boardContents;
		}


		[HttpGet("{id}")]
		public BoardContent GetBoardContent(int id)
		{
			BoardContent boardContent = _context.BoardContents
						.Where(item => item.Id == id)
						.FirstOrDefault();

            List<Comment> Comments = _context.Comments
                .Where(item => item.BoardContentId == id)
                .OrderBy(item => item.Id)
                .ToList();

            //foreach (Comment p in Comments)
            //{
            //    boardContent.Comments.Add(p);
            //}

            return boardContent;
		}

		// Update
		[HttpPut]
		public bool UpdateBoardContent([FromBody] BoardContent boardContent)
		{
			var findResult = _context.BoardContents
				.Where(x => x.Id == boardContent.Id)
				.FirstOrDefault();

			if (findResult == null)
				return false;

			findResult.Title = boardContent.Title;
			findResult.Content = boardContent.Content;
			_context.SaveChanges();

			return true;
		}

		// Delete
		[HttpDelete("{id}")]
		public bool DeleteBoardContent(int id)
		{
			var findResult = _context.BoardContents
						.Where(x => x.Id == id)
						.FirstOrDefault();

			if (findResult == null)
				return false;

			_context.BoardContents.Remove(findResult);
			_context.SaveChanges();

			return true;
		}
	}
}
