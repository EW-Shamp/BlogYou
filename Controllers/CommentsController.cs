using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BlogYou.Data;
using BlogYou.Models;
using Microsoft.AspNetCore.Identity;

namespace BlogYou.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BlogUser> _userManager;

        public CommentsController(ApplicationDbContext context, UserManager<BlogUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //GET: Comments
        public async Task<IActionResult> Index()
        {
            var comments = _context.Comments.Where(c => c.Moderated == null)
                                                        .Include(c => c.BlogUser)
                                                        .Include(c => c.Moderator)
                                                        .Include(c => c.Post);
            return View(await comments.ToListAsync());
        }

        //public async Task<IActionResult> UnmoderatedIndex()
        //{
        //     //==== Calls all comments that have not been moderated
        //   var unmoderatedComments = await _context.Comments.Where(c => c.Moderated == null).ToListAsync();
        //   return View("Index", unmoderatedComments);
        //}

        //public async Task<IActionResult> ModeratedIndex()
        //{
        //     //==== Calls all comments that have been moderated
        //     var moderatedComments = await _context.Comments.Where(c => c.Moderated != null).ToListAsync();
        //     return View("Index", moderatedComments);
        //}

        //public async Task<IactionResult> DeletedIndex()
        //{
            //==== Calls all comments that have been (probably soft) deleted
       // }



        // GET: Comments/Create
        //public IActionResult Create()
        //{
        //    ViewData["BlogUserId"] = new SelectList(_context.Users, "Id", "Id");
        //    ViewData["ModeratorId"] = new SelectList(_context.Users, "Id", "Id");
        //    ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Abstract");
        //    return View();
       // }

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PostId,Body")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                //var newComment = await _context.Comments.Include(c => c.Post).FirstOrDefaultAsync(c => c.Id == comment.Id);
                
                comment.BlogUserId = _userManager.GetUserId(User);
                comment.Created = DateTime.Now;
                _context.Add(comment);
                await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
                comment = await _context.Comments.Include(c => c.Post).FirstOrDefaultAsync(c => c.Id == comment.Id);

                return RedirectToAction("Details", "Posts", new { slug = comment.Post.Slug }, "commentSection");

            }



            return View(comment);

        }

        // GET: Comments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            ViewData["BlogUserId"] = new SelectList(_context.Users, "Id", "Id", comment.BlogUserId);
            ViewData["ModeratorId"] = new SelectList(_context.Users, "Id", "Id", comment.ModeratorId);
            ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Abstract", comment.PostId);
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Body")] Comment comment)
        {
            if (id != comment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var newComment = await _context.Comments.Include(c => c.Post).FirstOrDefaultAsync(c => c.Id == comment.Id);
                try
                {
                    newComment.Body = comment.Body;
                    newComment.Updated = DateTime.Now;
                    
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Posts", new { slug = newComment.Post.Slug }, "commentSection");
            }
            ViewData["BlogUserId"] = new SelectList(_context.Users, "Id", "Id", comment.BlogUserId);
            ViewData["ModeratorId"] = new SelectList(_context.Users, "Id", "Id", comment.ModeratorId);
            ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Abstract", comment.PostId);
            return View(comment);
        }

        //Moderate Comments
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Moderate(int id, [Bind("Id,Body,ModeratedBody,ModerationType")] Comment comment)
        {
            if (id != comment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var newComment = await _context.Comments.Include(c => c.Post).FirstOrDefaultAsync(c => c.Id == comment.Id);
                try
                {
                    newComment.ModeratedBody = comment.ModeratedBody;
                    newComment.ModerationType = comment.ModerationType;
                    newComment.Moderated = DateTime.Now;
                    newComment.ModeratorId = _userManager.GetUserId(User);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Posts", new { slug = newComment.Post.Slug }, "commentsSection");
            }
            return View(comment);
        }

        // GET: Comments/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var comment = await _context.Comments
        //        .Include(c => c.BlogUser)
        //        .Include(c => c.Moderator)
        //        .Include(c => c.Post)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (comment == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(comment);
        //}

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string slug)
        {
            var comment = await _context.Comments.FindAsync(id);
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Posts", new { slug }, "commentsSection");
        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.Id == id);
        }
    }
}
