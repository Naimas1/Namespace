using Google.Protobuf.WellKnownTypes;
using MiNET.UI;
using System.Collections.Generic;
using System.Diagnostics;

internal class model
{
}
< h1 > Info List </ h1 >

< Form asp - action = "Index" method = "get" >
    < Input type = "text" name = "searchString" Value = "@ViewData["CurrentFilter"]" />
    <button type="submit">Search</button>
    <button type="button" onclick="location.href='@Url.Action("Index")'">Clear</button>
</form>

<div [DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
classprivate string GetDebuggerDisplay()
{
    return ToString();
}= "row" >
    @foreach(var item in Model)
    {
        < div class= "card" style = "width: 18rem;" >
            < img src = "~/Uploads/@item.ImagePath" class= "card-img-top" alt = "@item.FirstName @item.LastName" >
            < div class= "card-body" >
                < h5 class= "card-title" > @item.FirstName @item.LastName </ h5 >
                < p class= "card-text" > @item.Description </ p >
                < p class= "card-text" > Profession: @item.ProfessionName </ p >
                < p class= "card-text" > Rating: @item.Rating </ p >
                < div class= "star-rating" >
                    @for(int i = 1; i <= 5; i++)
                    {
                        < span class= "fa fa-star @(item.Rating >= i ? "checked" : "")" onclick = "rate(@item.Id, @i)" ></ span >
                    }
                </ div >
            </ div >
        </ div >
    }
</ div >

@section Scripts {
    <script>
        function rate(id, rating) {
            $.ajax({
                type: "POST",
                url: '@Url.Action("Rate", "Info")',
                data: { id: id, rating: rating },
                success: function() {
    location.reload();
}
            });
        }
    </ script >
}
