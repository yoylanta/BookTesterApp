let currentPage = 1;
let loading = false; 

$(document).ready(function () {

    loadBooks(true);

    $("#language, #seed, #likes, #reviews").on("change input", function () {
        loadBooks(true);
    });

    $('#likes').on('input change', function () {
        $('#likesValue').text($(this).val());
        loadBooks(true);
    });

    $('#randomSeedBtn').click(function () {
        const randomSeed = Math.floor(Math.random() * 999999999);
        $('#seed').val(randomSeed);
        loadBooks(true);
    });

    $('#tableViewBtn').click(function () {
        $('#tableViewBtn').addClass('active');
        $('#galleryViewBtn').removeClass('active');
        $('#tableView').removeClass('d-none');
        $('#galleryView').addClass('d-none');
    });
    $('#galleryViewBtn').click(function () {
        $('#galleryViewBtn').addClass('active');
        $('#tableViewBtn').removeClass('active');
        $('#galleryView').removeClass('d-none');
        $('#tableView').addClass('d-none');
    });

    $(window).scroll(function () {
        if (!loading && $(window).scrollTop() + $(window).height() >= $(document).height() - 100) {
            loadBooks(false);
        }
    });
});

function loadBooks(reset) {
    if (reset) {
        currentPage = 1;
        $('#booksTableBody').empty();
        $('#galleryView').empty();
    }
    loading = true;

    const language = $('#language').val();
    const seed = parseInt($('#seed').val());
    const avgLikes = parseFloat($('#likes').val());
    const avgReviews = parseFloat($('#reviews').val());

    $.ajax({
        url: '/Books/LoadBooks',
        method: 'GET',
        data: {
            language: language,
            seed: seed,
            avgLikes: avgLikes,
            avgReviews: avgReviews,
            page: currentPage
        },
        success: function (data) {
            if (data && data.length > 0) {
                data.forEach(book => {
                    addBookToTable(book);
                    addBookToGallery(book);
                });
                currentPage++;
            }
            loading = false;
        },
        error: function () {
            loading = false;
        }
    });
}

function escapeHtml(text) {
    return $('<div>').text(text).html();
}

function addBookToTable(book) {
    const row = `
          <tr>
            <td>${book.index}</td>
            <td>${book.isbn}</td>
            <td>${escapeHtml(book.title)}</td>
            <td>${book.authors.join(', ')}</td>
            <td>${escapeHtml(book.publisher)}</td>
            <td>${book.likes}</td>
            <td>
              <button class="btn btn-sm btn-info toggle-details" data-id="details-${book.index}">
                <i class="fas fa-plus"></i>
              </button>
            </td>
          </tr>
        `;

    let reviewsHtml = '';
    if (book.reviews && book.reviews.length > 0) {
        console.log('book.reviews', book.reviews);
        console.log('escapeHtml(r.text)', escapeHtml('r.text'));
        reviewsHtml = book.reviews.map(r =>
            `<p>"${escapeHtml(r.text)}"<br><em> - ${escapeHtml(r.author)}</em></p>`).join('');
    } else {
        reviewsHtml = `<p>No reviews</p>`;
    }

    const detailsRow = `
          <tr class="details-row" id="details-${book.index}" style="display: none;">
            <td colspan="7">
              <div class="d-flex">
                <img src="${book.coverUrl}" alt="Cover" class="book-cover me-3" />
                <div>
                  <h5>${escapeHtml(book.title)}</h5>
                  <p><strong>By:</strong> ${book.authors.join(', ')}</p>
                  <p><strong>Publisher:</strong> ${escapeHtml(book.publisher)}</p>
                  <div>${reviewsHtml}</div>
                </div>
              </div>
            </td>
          </tr>
        `;

    $('#booksTableBody').append(row);
    $('#booksTableBody').append(detailsRow);

    $('#booksTableBody').find(`button[data-id="details-${book.index}"]`).off('click').on('click', function () {
        const target = $(this).data('id');
        $(`#${target}`).toggle();
    });
}

function addBookToGallery(book) {
    let reviewsHtml = '';
    if (book.reviews && book.reviews.length > 0) {
        reviewsHtml = book.reviews.map(r =>
            `<p class="mb-1">"${escapeHtml(r.text)}"<br><em>– ${escapeHtml(r.author)}</em></p>`).join('');
    } else {
        reviewsHtml = `<p>No reviews</p>`;
    }

    const cardHtml = `
          <div class="col-sm-6 col-md-4 col-lg-3 mb-4">
            <div class="card h-100">
              <img src="${book.coverUrl}" class="card-img-top" alt="Cover">
              <div class="card-body">
                <h5 class="card-title">${escapeHtml(book.title)}</h5>
                <p class="card-text">
                  <strong>Author(s):</strong> ${book.authors.join(', ')}<br>
                  <strong>ISBN:</strong> ${book.isbn}<br>
                  <strong>Likes:</strong> ${book.likes}
                </p>
                <button class="btn btn-sm btn-outline-primary" 
                        type="button" data-bs-toggle="collapse"
                        data-bs-target="#collapse${book.index}">
                  <i class="fas fa-info-circle"></i> Details
                </button>
                <div class="collapse mt-2" id="collapse${book.index}">
                  <strong>Publisher:</strong> ${escapeHtml(book.publisher)}<br>
                  ${reviewsHtml}
                </div>
              </div>
            </div>
          </div>
        `;
    $('#galleryView').append(cardHtml);
}
