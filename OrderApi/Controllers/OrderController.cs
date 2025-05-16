using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orders.Domain.Entities;
using Orders.Domain.Interfaces;
using Orders.Domain.Models;
using Orders.Domain.Models.Filters;

namespace Orders.Api.Controllers
{
    [ApiController]
    [Route("pedidos")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Lista todos os pedidos com paginação
        /// </summary>
        /// <param name="filter">Dados para Paginação</param>
        /// <returns>Lista paginada de pedidos</returns>
        /// <response code="200">Retorna a lista de pedidos</response>
        /// <response code="401">Se o usuário não estiver autenticado</response>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<Order>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(PagedResult<Order>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<PagedResult<Order>>> GetAllAsync([FromQuery] OrderFilter filter)
        {
            return await _orderService.GetAllOrdersAsync(filter);
        }

        /// <summary>
        /// Obtém um pedido específico pelo ID
        /// </summary>
        /// <param name="id">ID do pedido (GUID)</param>
        /// <returns>Os detalhes do pedido</returns>
        /// <response code="200">Retorna o pedido solicitado</response>
        /// <response code="401">Se o usuário não estiver autenticado</response>
        /// <response code="404">Se o pedido não for encontrado</response>
        [HttpGet("{id:Guid}")]
        [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Order), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(Order), StatusCodes.Status404NotFound)]
        public async Task<Order> GetByIdAsync(Guid id)
        {
            return await _orderService.GetByIdAsync(id);
        }

        /// <summary>
        /// Cria um novo pedido no sistema
        /// </summary>
        /// <remarks>
        /// Exemplo de request:
        /// 
        /// POST /api/orders
        /// {
        ///     "Value": 200,
        ///     "Status": 3,
        ///     "Active": true
        /// }
        /// </remarks>
        /// <param name="order">Dados do pedido a ser criado</param>
        /// <returns>O pedido criado com status 201</returns>
        /// <response code="201">Retorna o pedido recém-criado</response>
        /// <response code="400">Se os dados do pedido forem inválidos</response>
        /// <response code="401">Se o usuário não estiver autenticado</response>
        [HttpPost]
        [ProducesResponseType(typeof(Order), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Order), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Order), StatusCodes.Status401Unauthorized)]
        [Route("", Name = "CreateAsync")]
        public async Task<ActionResult<Guid>> CreateAsync([FromBody] Order order)
        {
            Guid idOrder = await _orderService.CreateAsync(order);

            return CreatedAtRoute(
                routeName: "CreateAsync",
                routeValues: new { id = idOrder },
                value: idOrder);
        }

        /// <summary>
        /// Atualiza um pedido na api
        /// </summary>
        /// <remarks>
        /// Exemplo de request:
        /// 
        /// PUT /api/orders
        /// {
        ///     "Id":"87f8074f-f387-4bf1-8240-7e9fa20d20a8",
        ///     "Value": 200,
        ///     "Status": 3,
        ///     "Active": true
        /// }
        /// </remarks>
        /// <param name="order">Dados do pedido a ser atualizado</param>
        /// <returns>O pedido atualizado com status 201</returns>
        /// <response code="200">Retorna o pedido atualizado</response>
        /// <response code="400">Se os dados do pedido forem inválidos</response>
        /// <response code="401">Se o usuário não estiver autenticado</response>
        /// <response code="404">Se o pedido não existir</response>
        [HttpPut]
        [ProducesResponseType(typeof(Order), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Order), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Order), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(Order), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Order>> UpdateAsync([FromBody] Order order)
        {
            return Ok(await _orderService.UpdateAsync(order));
        }

        /// <summary>
        /// Exclui um pedido permanentemente do sistema (exclusão física)
        /// </summary>
        /// <param name="id">ID do pedido a ser excluído (GUID)</param>
        /// <returns>Resposta vazia (204 No Content)</returns>
        /// <response code="204">Exclusão realizada com sucesso</response>
        /// <response code="401">Se o usuário não estiver autenticado</response>
        /// <response code="403">Se o pedido estiver com status "Pago" (não pode ser excluído)</response>
        /// <response code="404">Se o pedido não for encontrado</response>
        [HttpDelete("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            await _orderService.DeleteAsync(id, false);

            return NoContent();
        }

        /// <summary>
        /// Marca um pedido como excluído (exclusão lógica)
        /// </summary>
        /// <remarks>
        /// A exclusão lógica preserva os dados no banco, marcando o campo `IsDeleted` como true.
        /// </remarks>
        /// <param name="id">ID do pedido a ser marcado como excluído (GUID)</param>
        /// <returns>Resposta vazia (204 No Content)</returns>
        /// <response code="204">Exclusão lógica realizada com sucesso</response>
        /// <response code="401">Se o usuário não estiver autenticado</response>
        /// <response code="404">Se o pedido não for encontrado</response>
        [HttpDelete("logical/{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteLogicalAsync(Guid id)
        {
            await _orderService.DeleteAsync(id, true);

            return NoContent();
        }
    }
}
