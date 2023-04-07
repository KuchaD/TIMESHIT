using System.Drawing;
using IdentityManager.Shared.Shared;
using Mediator;
using SpreadCheetah;
using SpreadCheetah.Styling;
using SpreadCheetah.Worksheets;
using TimeShitApp.Share;

namespace TimeShitApp.Application;

public sealed record GenerateTimeSheetRequest(IList<Time> Times, User User) : IRequest<Result<GenerateTimeSheetResponse>>;
public sealed record GenerateTimeSheetResponse(Stream Stream);

public sealed class GenerateTimeSheetHandler : IRequestHandler<GenerateTimeSheetRequest, Result<GenerateTimeSheetResponse>>
{
    public async ValueTask<Result<GenerateTimeSheetResponse>> Handle(GenerateTimeSheetRequest request, CancellationToken cancellationToken)
    {
        var totalHours = request.Times.Sum(x => x.Hours);
        var stream = new MemoryStream();
        await using (var spreadsheet = await Spreadsheet.CreateNewAsync(stream, cancellationToken: cancellationToken))
        {
            var worksheetOptions = new WorksheetOptions();
            worksheetOptions.Column(1).Width = 15;
            worksheetOptions.Column(2).Width = 15;
            worksheetOptions.Column(3).Width = 65;
            worksheetOptions.Column(4).Width = 10;
            worksheetOptions.Column(5).Width = 17;
            worksheetOptions.Column(6).Width = 7;
            
            var onlyBorder = new Style();
            onlyBorder.Border.Bottom.BorderStyle = BorderStyle.Thin;
            onlyBorder.Border.Top.BorderStyle = BorderStyle.Thin;
            onlyBorder.Border.Left.BorderStyle = BorderStyle.Thin;
            onlyBorder.Border.Right.BorderStyle = BorderStyle.Thin;
            
            var onlyBorderId = spreadsheet.AddStyle(onlyBorder);
            
            // A spreadsheet must contain at least one worksheet.
            await spreadsheet.StartWorksheetAsync("timesheet", worksheetOptions, cancellationToken);
            
            //Title
            var titleStyle = new Style();
            titleStyle.Fill.Color = Color.Gray;
            titleStyle.Font.Bold = true;
            titleStyle.Font.Size = 30;
            titleStyle.Alignment.Vertical = VerticalAlignment.Center;
            titleStyle.Alignment.Horizontal = HorizontalAlignment.Center;
            var titleStyleId = spreadsheet.AddStyle(titleStyle);
            var rowOptions = new RowOptions { Height = 50 };

            var rowTitle = new List<Cell>();
            spreadsheet.MergeCells("A1:F1");
            rowTitle.Add(new Cell("Timesheet", titleStyleId));
                
            await spreadsheet.AddRowAsync(rowTitle, rowOptions, cancellationToken);
            await spreadsheet.AddRowAsync(new []{new Cell(string.Empty)}, cancellationToken);
            
            //InfoRows
            var infoStyle = new Style();
            infoStyle.Fill.Color = Color.DarkSeaGreen;
            infoStyle.Border.Bottom.BorderStyle = BorderStyle.Thin;
            infoStyle.Border.Top.BorderStyle = BorderStyle.Thin;
            infoStyle.Border.Left.BorderStyle = BorderStyle.Thin;
            infoStyle.Border.Right.BorderStyle = BorderStyle.Thin;
            var infoStyleId = spreadsheet.AddStyle(infoStyle);
            
            var rowInfo1 = new List<Cell>();
            rowInfo1.Add(new Cell("Měsíc", infoStyleId));
            rowInfo1.Add(new Cell(request.Times.First().Date.ToString("MM.yyyy"), onlyBorderId));
            rowInfo1.Add(new Cell(string.Empty));
            rowInfo1.Add(new Cell(string.Empty));
            rowInfo1.Add(new Cell("Celkem hodin", infoStyleId));
            rowInfo1.Add(new Cell(totalHours, onlyBorderId));
            await spreadsheet.AddRowAsync(rowInfo1, cancellationToken);
            
            var rowInfo2 = new List<Cell>();
            rowInfo2.Add(new Cell("Jméno", infoStyleId));
            rowInfo2.Add(new Cell($"{request.User.Name} {request.User.Surname}", onlyBorderId));
            rowInfo2.Add(new Cell(string.Empty));
            rowInfo2.Add(new Cell(string.Empty));
            rowInfo2.Add(new Cell("Celkem MD", infoStyleId));
            rowInfo2.Add(new Cell(totalHours / 8, onlyBorderId));
            
            await spreadsheet.AddRowAsync(rowInfo2, cancellationToken);
            await spreadsheet.AddRowAsync(new []{new Cell(string.Empty)}, cancellationToken);
            //Header
            var headerStyle = new Style();
            headerStyle.Fill.Color = Color.Orange;
            var headerStyleId = spreadsheet.AddStyle(headerStyle);
            
            var rowHeader = new List<Cell>();
            rowHeader.Add(new Cell("Datum", headerStyleId));
            rowHeader.Add(new Cell("Hodiny", headerStyleId));
            rowHeader.Add(new Cell("Činnost", headerStyleId));
            rowHeader.Add(new Cell("Projekt", headerStyleId));
            rowHeader.Add(new Cell(string.Empty, headerStyleId));
            rowHeader.Add(new Cell(string.Empty, headerStyleId));
            
            await spreadsheet.AddRowAsync(rowHeader, cancellationToken);
            var start = 7;
            for (var i = 0; i < request.Times.Count; i++)
            {
                var numerRow = i + start;
                var merge = $"D{numerRow}:F{numerRow}";
                spreadsheet.MergeCells(merge);
                
                var time = request.Times[i];
                // Cells are inserted row by row.
                var row = new List<Cell>();
                row.Add(new Cell(time.Date.ToString("dd.MM.yyyy"), onlyBorderId));
                row.Add(new Cell(time.Hours, onlyBorderId));
                row.Add(new Cell(time.Task, onlyBorderId));
                row.Add(new Cell(time.Project, onlyBorderId));
                row.Add(new Cell(string.Empty, onlyBorderId));
                row.Add(new Cell(string.Empty, onlyBorderId));
                
                // Rows are inserted from top to bottom.
                await spreadsheet.AddRowAsync(row, cancellationToken);
            }
            var summaryStyle = new Style();
            summaryStyle.Fill.Color = Color.Tan;
            summaryStyle.Border.Bottom.BorderStyle = BorderStyle.Thin;
            summaryStyle.Border.Top.BorderStyle = BorderStyle.Thin;
            summaryStyle.Border.Left.BorderStyle = BorderStyle.Thin;
            summaryStyle.Border.Right.BorderStyle = BorderStyle.Thin;
            var summaryStyleId = spreadsheet.AddStyle(summaryStyle);
            
            var rowSummary = new List<Cell>();
            rowSummary.Add(new Cell("Celkem", summaryStyleId));
            rowSummary.Add(new Cell(totalHours, summaryStyleId));
            await spreadsheet.AddRowAsync(rowSummary, cancellationToken);
            
            await spreadsheet.AddRowAsync(new []{new Cell(string.Empty)}, cancellationToken);
            
            var mergeSummary = $"D{spreadsheet.NextRowNumber}:E{spreadsheet.NextRowNumber}";
            spreadsheet.MergeCells(mergeSummary);
            await spreadsheet.AddRowAsync(new []
            {
                new Cell($"Datum schválení: {DateTime.Now.ToString("dd.MM.yyyy")}")
            }, cancellationToken);
            
            // Remember to call Finish before disposing.
            // This is important to properly finalize the XLSX file.
            await spreadsheet.FinishAsync();
        }
        
        return new GenerateTimeSheetResponse(stream);
    }
}