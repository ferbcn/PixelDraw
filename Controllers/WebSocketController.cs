﻿using System.Diagnostics;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWebApplication.Data;
using MyWebApplication.Models;
namespace MyWebApplication.Controllers;


public class WebSocketController : Controller
{
    // all users need to access the _sockets list for broadcasating to other useres
    private static List<WebSocket> _sockets = new List<WebSocket>();
    
    private MyWebApplicationContext _context;

    public WebSocketController(MyWebApplicationContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View("Index");
    }

    [HttpGet("/ws")]
    public async Task Get()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            await Echo(webSocket);
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }

    private static async Task Echo(WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];
        var receiveResult = await webSocket.ReceiveAsync(
            new ArraySegment<byte>(buffer), CancellationToken.None);

        while (!receiveResult.CloseStatus.HasValue)
        {
            await webSocket.SendAsync(
                new ArraySegment<byte>(buffer, 0, receiveResult.Count),
                receiveResult.MessageType,
                receiveResult.EndOfMessage,
                CancellationToken.None);

            receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);
        }

        await webSocket.CloseAsync(
            receiveResult.CloseStatus.Value,
            receiveResult.CloseStatusDescription,
            CancellationToken.None);
    }

    [HttpGet("/wsclick")]
    public async Task GetClick()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            _sockets.Add(webSocket); // Add the new WebSocket to the list.
            await ClickCell(webSocket);
            _sockets.Remove(webSocket); // Remove the WebSocket when it's done
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }

    public async Task ClickCell(WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];
        WebSocketReceiveResult receiveResult;

        do
        {
            receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);

            var receivedString = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);
            
            var parts = receivedString.Split(":");
            string color = "#ffffff";
            try
            {
                color = parts[3];
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e} , receivedString: {receivedString}");
                continue;
            }
            if (parts.Length < 3 ||
                !int.TryParse(parts[0], out int boardId) ||
                !int.TryParse(parts[1], out int i) ||
                !int.TryParse(parts[2], out int j))
            {
                // Malformed request, ignore and continue with the next iteration
                continue;
            }
            if (color == "nuke")
            {
                List<Cell> cells = _context.Cell.Where(c => c.BoardId == boardId).ToList();
                foreach (var cell in cells)
                {
                    _context.Remove(cell);
                }
                await _context.SaveChangesAsync();
            }
            else
            {
                Cell? currentCell = await _context.Cell.FirstOrDefaultAsync(c => c.X == j && c.Y == i && c.BoardId == boardId);
                if (currentCell == null){
                    Cell newCell = new() { X = j, Y = i, Color = color, BoardId = boardId };
                    _context.Add(newCell);
                }
                else
                {
                    currentCell.Color = color;
                }
                await _context.SaveChangesAsync();
            }

            string messageToSend;
            messageToSend = $"{boardId}:{i}:{j}:{color}";
            
            // After saving changes to the database, broadcast the message to all connected clients.
            var sendBuffer = Encoding.UTF8.GetBytes(messageToSend);
            foreach (var socket in _sockets)
            {
                if (socket.State == WebSocketState.Open)
                {
                    await socket.SendAsync(
                        new ArraySegment<byte>(sendBuffer, 0, sendBuffer.Length),
                        receiveResult.MessageType,
                        receiveResult.EndOfMessage,
                        CancellationToken.None);
                }
            }

        } while (!receiveResult.CloseStatus.HasValue);

        await webSocket.CloseAsync(
            receiveResult.CloseStatus.Value,
            receiveResult.CloseStatusDescription,
            CancellationToken.None);
    }
}
